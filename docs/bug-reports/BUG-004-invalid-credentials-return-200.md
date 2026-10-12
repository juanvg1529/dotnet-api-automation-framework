# BUG-004 — Invalid credentials return 200 OK

| Field | Value |
|---|---|
| **Severity** | Minor |
| **Priority** | Medium |
| **Status** | Open |
| **Endpoint** | `POST /auth` |
| **Found by** | Automated scenario `Invalid credentials are rejected` (`Authentication.feature`) |
| **Date** | 2026-10-11 |

## Summary
A failed login answers `200 OK` with `{"reason":"Bad credentials"}` instead of an error status. No token is issued, so access control itself works.

## Steps to reproduce
```bash
curl -i -X POST https://restful-booker.herokuapp.com/auth \
  -H "Content-Type: application/json" -H "Accept: application/json" \
  -d '{"username":"admin","password":"wrong"}'
```

## Expected result
`401 Unauthorized`.

## Actual result
```
HTTP/1.1 200 OK

{"reason":"Bad credentials"}
```

## Impact
- Clients that rely on the status code treat a failed login as a success and break later, when the token is missing.
- Security monitoring that counts failed logins (`401`) misses brute-force attempts.
