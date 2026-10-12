# BUG-003 — Deleting a booking returns 201 Created

| Field | Value |
|---|---|
| **Severity** | Minor |
| **Priority** | Low |
| **Status** | Open |
| **Endpoint** | `DELETE /booking/{id}` |
| **Found by** | Automated scenario `Delete a booking with a valid token` (`DeleteBooking.feature`) |
| **Date** | 2026-10-11 |

## Summary
A successful delete answers `201 Created`, a status code meant for resource creation. The booking is deleted correctly: a later `GET` returns `404`.

## Steps to reproduce
```bash
TOKEN=$(curl -s -X POST https://restful-booker.herokuapp.com/auth \
  -H "Content-Type: application/json" \
  -d '{"username":"admin","password":"password123"}' | jq -r .token)

curl -i -X DELETE https://restful-booker.herokuapp.com/booking/<id> -H "Cookie: token=$TOKEN"
curl -i https://restful-booker.herokuapp.com/booking/<id>
```

## Expected result
`204 No Content` (or `200 OK` with a body), per RFC 9110 §9.3.5.

## Actual result
```
DELETE → HTTP/1.1 201 Created   body: "Created"
GET    → HTTP/1.1 404 Not Found
```

## Impact
Low functional impact, but clients and generated SDKs that follow HTTP semantics may treat the answer as unexpected. It also suggests the API contract isn't checked against HTTP conventions.
