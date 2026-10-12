Feature: Search bookings
  Bookings can be filtered by guest name.

  @regression
  Scenario: Find a booking by guest first and last name
    Given an existing booking
    When I search bookings by the guest's first and last name
    Then the response status code should be 200
    And the search results should include the booking

  @regression
  Scenario: Searching for a guest with no bookings returns an empty list
    When I search bookings for a guest that does not exist
    Then the response status code should be 200
    And the search results should be empty
