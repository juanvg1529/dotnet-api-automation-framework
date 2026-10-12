# Traceability Matrix

Each requirement is linked to the scenarios that verify it and to any defect found. Requirements are derived from the [Restful-Booker API documentation](https://restful-booker.herokuapp.com/apidoc/index.html) and common REST conventions (RFC 9110).

| ID | Requirement | Scenario(s) | Feature | Status |
|---|---|---|---|---|
| REQ-01 | The API exposes a health check | The API is up and running | `HealthCheck` | ✅ Pass |
| REQ-02 | Valid credentials return an auth token | Valid credentials return a token | `Authentication` | ✅ Pass |
| REQ-03 | Invalid credentials are rejected and no token is issued | Invalid credentials are rejected (3 cases) | `Authentication` | ⚠️ Pass with defect [BUG-004](bug-reports/BUG-004-invalid-credentials-return-200.md) |
| REQ-04 | A valid booking is created and stored exactly as sent | Create a booking with valid data; Create bookings with different payment and extras (3 cases) | `CreateBooking` | ✅ Pass |
| REQ-05 | Responses follow a stable contract | Create a booking with valid data; Fully update a booking | `CreateBooking`, `UpdateBooking` | ✅ Pass |
| REQ-06 | Bookings missing required fields are rejected with `400` | Creating a booking without a required field (3 cases) | `CreateBooking` | ❌ Defect [BUG-001](bug-reports/BUG-001-missing-required-field-returns-500.md) |
| REQ-07 | A stay must end after it starts | A booking with checkout before checkin is accepted | `CreateBooking` | ❌ Defect [BUG-002](bug-reports/BUG-002-checkout-before-checkin-accepted.md) |
| REQ-08 | Authorised users can fully update a booking | Fully update a booking with a valid token | `UpdateBooking` | ✅ Pass |
| REQ-09 | Authorised users can partially update a booking | Partially update a booking with a valid token | `UpdateBooking` | ✅ Pass |
| REQ-10 | Updates without a valid token are forbidden | Updating without a token; Updating with an invalid token | `UpdateBooking` | ✅ Pass |
| REQ-11 | Authorised users can delete a booking, and it no longer exists | Delete a booking with a valid token | `DeleteBooking` | ⚠️ Pass with defect [BUG-003](bug-reports/BUG-003-delete-returns-201-created.md) |
| REQ-12 | Deletes without a token are forbidden and change nothing | Deleting a booking without a token is forbidden | `DeleteBooking` | ✅ Pass |
| REQ-13 | Bookings can be searched by guest name | Find a booking by guest first and last name | `SearchBooking` | ✅ Pass |
| REQ-14 | Searching for an unknown guest returns an empty list | Searching for a guest with no bookings | `SearchBooking` | ✅ Pass |
| REQ-15 | Main operations answer within the agreed time (3,000 ms) | Time check on health, auth, create, update, delete and search scenarios | several | ✅ Pass |

**Coverage:** 15 requirements, 21 scenarios, 4 open defects.
