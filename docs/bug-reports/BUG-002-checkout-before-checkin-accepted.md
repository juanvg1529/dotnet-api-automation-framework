# BUG-002 — A booking with checkout before checkin is accepted

| Field | Value |
|---|---|
| **Severity** | Major |
| **Priority** | Medium |
| **Status** | Open |
| **Endpoint** | `POST /booking` (also `PUT` / `PATCH /booking/{id}`, not yet automated) |
| **Found by** | Automated scenario `A booking with checkout before checkin is accepted` (`CreateBooking.feature`) |
| **Date** | 2026-10-11 |

## Summary
The API stores a booking whose checkout date is earlier than its checkin date: a stay that cannot exist.

## Steps to reproduce
```bash
curl -i -X POST https://restful-booker.herokuapp.com/booking \
  -H "Content-Type: application/json" -H "Accept: application/json" \
  -d '{"firstname":"Qa","lastname":"Evidence","totalprice":100,"depositpaid":true,
       "bookingdates":{"checkin":"2026-11-10","checkout":"2026-11-01"},
       "additionalneeds":"Breakfast"}'
```

## Expected result
`400 Bad Request` explaining that checkout must be after checkin.

## Actual result
```
HTTP/1.1 200 OK

{"bookingid":2170,"booking":{"firstname":"Qa","lastname":"Evidence","totalprice":100,
 "depositpaid":true,"bookingdates":{"checkin":"2026-11-10","checkout":"2026-11-01"},
 "additionalneeds":"Breakfast"}}
```

## Impact
Invalid data reaches the system of record. Anything downstream (availability, invoicing at `totalprice` per night, reports) may break or produce wrong numbers.

## Suggested follow-up
Add the same check to `PUT` and `PATCH`, and cover them with scenarios.
