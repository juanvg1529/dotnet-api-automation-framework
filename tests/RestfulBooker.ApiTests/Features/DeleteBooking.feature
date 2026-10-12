Feature: Delete booking
  Cancelling a booking removes it; only authenticated users can do it.

  Background:
    Given an existing booking

  @regression @known-defect
  Scenario: Delete a booking with a valid token
    Given I am authenticated
    When I delete the booking
    # Expected: 204 No Content (or 200). Actual: 201 Created.
    Then the response status code should be 201
    And the booking should no longer exist

  @negative
  Scenario: Deleting a booking without a token is forbidden
    When I delete the booking
    Then the response status code should be 403
    And the booking should still exist
