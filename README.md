# .NET API Automation Framework

[![API Tests](https://github.com/juanvg1529/dotnet-api-automation-framework/actions/workflows/api-tests.yml/badge.svg)](https://github.com/juanvg1529/dotnet-api-automation-framework/actions/workflows/api-tests.yml)
[![Allure Report](https://img.shields.io/badge/report-Allure-orange)](https://juanvg1529.github.io/dotnet-api-automation-framework/)
![.NET 10](https://img.shields.io/badge/.NET-10-512BD4)
![Reqnroll](https://img.shields.io/badge/BDD-Reqnroll-2ea44f)

A BDD API test automation framework in **C# / .NET 10** for the public [Restful-Booker](https://restful-booker.herokuapp.com/apidoc/index.html) hotel booking API. It shows how I structure API automation in real projects: a reusable client layer, readable Gherkin scenarios, isolated test data, schema validation, parallel runs, CI and a published report.

**→ [Latest Allure report](https://juanvg1529.github.io/dotnet-api-automation-framework/)** · **[Test plan](docs/test-plan.md)** · **[Traceability matrix](docs/traceability-matrix.md)** · **[Bug reports](docs/bug-reports)**

## Stack

| Concern | Tool |
|---|---|
| Language / runtime | C# 14, .NET 10 |
| BDD | [Reqnroll](https://reqnroll.net/) (the successor of SpecFlow) |
| Test runner / assertions | NUnit |
| HTTP client | RestSharp |
| Test data | Bogus |
| Contract checks | JSON Schema (JsonSchema.Net) |
| Reporting | Allure |
| CI/CD | GitHub Actions → GitHub Pages |

## Coverage

21 scenarios across 6 features:

| Feature | What is verified |
|---|---|
| Health check | API availability before trusting any other result |
| Authentication | Token issued for valid credentials; wrong, unknown and empty credentials rejected |
| Create booking | Valid bookings stored exactly as sent, response schema, data-driven variations, **documented API defects** |
| Update booking | Full (PUT) and partial (PATCH) updates persist; missing or invalid token → 403 |
| Delete booking | Deleted booking returns 404 afterwards; deleting without a token is forbidden and leaves the booking intact |
| Search bookings | Filtering by guest name finds the booking; unknown guest returns an empty list |
| Response time | Main operations answer within the agreed limit (3,000 ms by default, configurable per environment) |

Tags let each pipeline choose its scope: `@smoke` (fast gate on every PR), `@regression`, `@negative`, `@known-defect`.

## Architecture

```
src/RestfulBooker.Api/              ← reusable API client library (no test code)
├── Clients/                        ApiClient (shared RestClient), AuthClient, BookingClient
├── Models/                         Booking, BookingDates, auth contracts
└── Configuration/                  ApiSettings: appsettings + per-environment file + env vars

tests/RestfulBooker.ApiTests/
├── Features/                       Gherkin scenarios: the living documentation
├── StepDefinitions/                thin glue: call a client, assert on the response
├── Hooks/                          shared client setup, DI registration, cleanup, report attachments
├── Support/                        BookingBuilder (Bogus), SchemaValidator, AuthTokenCache, ScenarioData, QualitySettings
└── Schemas/                        JSON Schemas for response contracts

docs/                               ← QA documentation
├── test-plan.md                    scope, ISO/IEC 25010 quality characteristics, approach, entry/exit criteria, risks
├── traceability-matrix.md          requirement → scenario → defect
└── bug-reports/                    one report per defect, with curl steps to reproduce
```

```
Feature file ──► Step definitions ──► BookingClient / AuthClient ──► ApiClient (RestSharp) ──► API
                       │                                                     
                       └── ScenarioData (per-scenario state, injected by Reqnroll)
```

## Design decisions

- **Client library separated from tests.** `RestfulBooker.Api` knows nothing about NUnit or Reqnroll, so it could be reused by a UI framework (for example to create test data via API) or by performance scripts.
- **Clients return raw responses; tests decide what "correct" means.** No assertions hide inside the client, so negative tests can check 403/404/500 responses as easily as happy paths.
- **Every scenario creates its own data.** Bogus generates unique guests, which lets features run **in parallel** against a shared public API without colliding. An `AfterScenario` hook deletes whatever the scenario created.
- **The token is requested once per run.** `AuthTokenCache` shares it across parallel scenarios (thread-safe), instead of calling `POST /auth` before every write.
- **Contract and data checks are separate.** JSON Schema validates the *shape* of a response; record equality validates the *values* match what was sent, both in the response and after reading it back with `GET`.
- **Known defects are documented, not hidden.** Restful-Booker has real bugs (see below). Instead of skipping those cases, `@known-defect` scenarios assert today's behaviour and explain the expected one in a comment. If the API gets fixed, they fail on purpose, which is the signal to update them.
- **Non-functional checks live next to functional ones.** Every request is timed; main scenarios assert the response arrives within a limit from configuration (`Quality:MaxResponseTimeMs`), so CI and a slower staging environment can use different limits without code changes.
- **Configuration without secrets in code.** `appsettings.json` → `appsettings.{TEST_ENV}.json` → `RB_*` environment variables (used by CI with repository secrets).

## Defects found in the API

| ID | Behaviour | Expected | Actual | Severity |
|---|---|---|---|---|
| [BUG-001](docs/bug-reports/BUG-001-missing-required-field-returns-500.md) | Create booking without `firstname`, `lastname` or `bookingdates` | `400 Bad Request` with a validation message | `500 Internal Server Error` | Major |
| [BUG-002](docs/bug-reports/BUG-002-checkout-before-checkin-accepted.md) | Create booking with checkout before checkin | `400 Bad Request` | `200 OK`, impossible stay stored | Major |
| [BUG-003](docs/bug-reports/BUG-003-delete-returns-201-created.md) | Delete an existing booking | `204 No Content` / `200 OK` | `201 Created` | Minor |
| [BUG-004](docs/bug-reports/BUG-004-invalid-credentials-return-200.md) | Invalid credentials on `POST /auth` | `401 Unauthorized` | `200 OK` with `{"reason":"Bad credentials"}` | Minor |

## Running locally

Requires the [.NET 10 SDK](https://dotnet.microsoft.com/download).

```bash
dotnet test                                  # all scenarios
dotnet test --filter "Category=smoke"        # only the smoke suite
dotnet test --filter "Category=regression"
```

Run against another environment:

```bash
TEST_ENV=staging dotnet test                 # loads appsettings.staging.json
RB_Api__BaseUrl=http://localhost:3001 dotnet test
RB_Quality__MaxResponseTimeMs=5000 dotnet test   # looser time limit
```

View the Allure report locally (needs Java):

```bash
npx allure-commandline generate tests/RestfulBooker.ApiTests/bin/Debug/net10.0/allure-results --clean --single-file -o allure-report
open allure-report/index.html
```

> **Note on flakiness:** Restful-Booker is a shared public sandbox that wipes its data every 10 minutes. A scenario running exactly at reset time can lose its booking and fail. Unique data and short scenarios keep this rare; the next step below (a local container in CI) removes it.

## CI/CD

[`.github/workflows/api-tests.yml`](.github/workflows/api-tests.yml):

- Runs on every push and pull request to `main`, on a weekday schedule (catches API changes with no commits), and on demand with an optional tag filter.
- Publishes Allure and TRX results as artifacts.
- On `main`, generates the Allure report and deploys it to GitHub Pages.

## Next steps

- [ ] Allure history between runs (trend charts)
- [ ] Docker image to run the suite with one command
- [ ] Run the suite against a local Restful-Booker container in CI

---

Built by **Juan Diego Vera Godoy**, QA Automation Engineer / SDET · [LinkedIn](https://www.linkedin.com/in/juan-diego-vera-godoy-3949a6183) · See also: [FHIR R4 API Automation](https://github.com/juanvg1529/fhir-api-automation) (healthcare interoperability testing)
