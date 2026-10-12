# BUG-001 — Creating a booking without a required field returns 500 instead of 400

| Field | Value |
|---|---|
| **Severity** | Major |
| **Priority** | High |
| **Status** | Open |
| **Endpoint** | `POST /booking` |
| **Found by** | Automated scenario `Creating a booking without a required field` (`CreateBooking.feature`) |
| **Date** | 2026-10-11 |

## Summary
When the request body is missing `firstname`, `lastname` or `bookingdates`, the API crashes with `500 Internal Server Error` instead of rejecting the input with `400 Bad Request` and a message.

## Steps to reproduce
```bash
curl -i -X POST https://restful-booker.herokuapp.com/booking \
  -H "Content-Type: application/json" -H "Accept: application/json" \
  -d '{"lastname":"Evidence","totalprice":100,"depositpaid":true,
       "bookingdates":{"checkin":"2026-11-01","checkout":"2026-11-10"},
       "additionalneeds":"Breakfast"}'
```

## Expected result
`400 Bad Request` with a body that says which field is missing.

## Actual result
```
HTTP/1.1 500 Internal Server Error

Internal Server Error
```

## Impact
- Clients can't tell their own mistake (4xx) from a server failure (5xx), so they may retry requests that will never succeed.
- 5xx responses trigger false alerts in monitoring and suggest missing input validation on the server.

## Notes
Reproduced with `firstname`, `lastname` and `bookingdates`. `additionalneeds` is optional and can be omitted without error.
