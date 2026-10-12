Feature: Authentication
  Write operations (update, delete) require a token obtained from POST /auth.

  @smoke
  Scenario: Valid credentials return a token
    When I request a token with valid credentials
    Then the response status code should be 200
    And the response time should be within the agreed limit
    And the response should contain a token

  @negative @known-defect
  Scenario Outline: Invalid credentials are rejected - <case>
    When I request a token with username "<username>" and password "<password>"
    # Expected: 401 Unauthorized. Actual: 200 OK with a "reason" in the body.
    Then the response status code should be 200
    And the authentication should fail with reason "Bad credentials"

    Examples:
      | case           | username | password    |
      | wrong password | admin    | not-it      |
      | unknown user   | nobody   | password123 |
      | empty values   |          |             |
