Feature: Health check
  The suite verifies the API is reachable before trusting any other result.

  @smoke
  Scenario: The API is up and running
    When I ping the API
    Then the response status code should be 201
    And the response time should be within the agreed limit
