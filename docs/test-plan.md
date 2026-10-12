# Test Plan — Restful-Booker API

| | |
|---|---|
| **Product** | Restful-Booker hotel booking API (`https://restful-booker.herokuapp.com`) |
| **Version under test** | Public sandbox, as available (no versioning exposed) |
| **Author** | Juan Diego Vera Godoy |
| **Last updated** | 2026-10-11 |
| **Structure** | Lightweight version of ISO/IEC/IEEE 29119-3 |

## 1. Objective

Give fast, repeatable evidence that the booking API:

1. lets authorised users create, read, update, delete and search bookings correctly,
2. rejects unauthorised or invalid requests in a safe way,
3. keeps a stable response contract,
4. answers within an acceptable time.

## 2. Scope

### In scope

| Area | Endpoints |
|---|---|
| Health | `GET /ping` |
| Authentication | `POST /auth` |
| Bookings | `GET /booking`, `GET /booking/{id}`, `POST /booking`, `PUT /booking/{id}`, `PATCH /booking/{id}`, `DELETE /booking/{id}` |

### Out of scope

- Load, stress and soak testing: a shared public sandbox is not the place for them. Planned with k6 against a local container.
- Security testing beyond access control (injection, rate limiting, TLS configuration).
- XML and URL-encoded payloads: the suite covers JSON only.
- Basic-auth login (`Authorization: Basic`): the suite covers token cookies only.

## 3. Quality characteristics (ISO/IEC 25010)

| Characteristic | Sub-characteristic | How it is verified |
|---|---|---|
| Functional suitability | Correctness, completeness | Stored data equals the data sent (checked in the response **and** by reading it back with `GET`) |
| Security | Confidentiality, integrity | Write operations without a token or with an invalid one must return `403` and change nothing |
| Compatibility | Interoperability | Response bodies validated against JSON Schemas |
| Performance efficiency | Time behaviour | Main operations must answer within the agreed limit (default 3,000 ms, configurable per environment) |
| Reliability | Fault tolerance | Invalid input must be rejected with `4xx`, never crash with `5xx` |

## 4. Approach

- **Level:** API / integration, black box.
- **Techniques:**
  - equivalence partitioning (valid / invalid credentials, deposit paid true / false),
  - boundary and invalid values (missing required fields, checkout before checkin),
  - state-based checks (exists → deleted → `404`).
- **BDD:** scenarios are written in Gherkin (Reqnroll) so the business can read them; they are the living documentation of expected behaviour.
- **Test data:** generated per scenario with Bogus (unique guest names), so scenarios are independent, can run in parallel, and clean up after themselves.
- **Known defects:** asserted as *current* behaviour and tagged `@known-defect`, with the expected behaviour in a comment. When the API is fixed, they fail on purpose so the expectation gets updated.

### Suites (tags)

| Tag | Purpose | When it runs |
|---|---|---|
| `@smoke` | Is the API usable at all? | Every pull request (fast gate) |
| `@regression` | Full functional coverage | Every push to `main`, every weekday morning |
| `@negative` | Invalid and unauthorised requests | With regression |
| `@known-defect` | Documents open defects | With regression |

## 5. Environment

| Item | Value |
|---|---|
| Base URL | `https://restful-booker.herokuapp.com` (override: `RB_Api__BaseUrl`) |
| Credentials | Public demo credentials (override: repository secrets `RB_USERNAME`, `RB_PASSWORD`) |
| Runner | GitHub Actions, `ubuntu-latest`, .NET 10 |
| Reporting | Allure (published to GitHub Pages), TRX artifacts |

**Environment risk:** the sandbox is shared and **wipes its data every 10 minutes**. A scenario running exactly at reset time can lose its booking.

## 6. Entry and exit criteria

**Entry**

- `GET /ping` returns `201`.
- Valid credentials return a token.

**Exit (release recommendation)**

- 100% of `@smoke` scenarios pass.
- No failures in `@regression` other than `@known-defect` scenarios whose behaviour changed (these are reviewed, not ignored).
- Every open defect is logged in `docs/bug-reports/`.

## 7. Risks and mitigation

| Risk | Impact | Mitigation |
|---|---|---|
| Shared sandbox data reset every 10 min | Intermittent false failures | Unique data per scenario, short scenarios; next step: run the API in a local container in CI |
| Other users modify or delete data | False failures | Each scenario creates and owns its data |
| Public API latency varies | Flaky time checks | Generous default limit, configurable per environment |
| API behaviour changes without notice | Suite goes red with no code change | Weekday scheduled run detects it early |

## 8. Deliverables

- Automated suite: [`tests/RestfulBooker.ApiTests`](../tests/RestfulBooker.ApiTests)
- Traceability matrix: [`traceability-matrix.md`](traceability-matrix.md)
- Defect reports: [`bug-reports/`](bug-reports)
- Execution report: [Allure on GitHub Pages](https://juanvg1529.github.io/dotnet-api-automation-framework/)
