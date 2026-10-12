Feature: Update booking
  Guests can change their booking; the API must only allow it with a valid token.

  Background:
    Given an existing booking

  @regression
  Scenario: Fully update a booking with a valid token
    Given I am authenticated
    When I replace the booking with new valid data
    Then the response status code should be 200
    And the response time should be within the agreed limit
    And the response should match the "booking" schema
    And the response should contain the expected booking
    And the booking can be retrieved with the same data

  @regression
  Scenario: Partially update a booking with a valid token
    Given I am authenticated
    When I change the first name to "Juan Diego" and the total price to 1250
    Then the response status code should be 200
    And the response time should be within the agreed limit
    And the booking can be retrieved with the same data

  @negative
  Scenario: Updating a booking without a token is forbidden
    When I replace the booking with new valid data
    Then the response status code should be 403

  @negative
  Scenario: Updating a booking with an invalid token is forbidden
    Given I use the token "invalid-token-123"
    When I replace the booking with new valid data
    Then the response status code should be 403
