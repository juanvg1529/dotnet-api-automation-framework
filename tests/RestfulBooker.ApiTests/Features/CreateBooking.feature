Feature: Create booking
  As a hotel guest
  I want to create a booking
  So that my stay is reserved with the right dates and options

  @smoke
  Scenario: Create a booking with valid data
    Given a valid booking
    When I create the booking
    Then the response status code should be 200
    And the response should match the "created-booking" schema
    And the created booking should match the request
    And the booking can be retrieved with the same data

  @regression
  Scenario Outline: Create bookings with different payment and extras - <deposit> / <needs>
    Given a valid booking with deposit paid "<deposit>" and additional needs "<needs>"
    When I create the booking
    Then the response status code should be 200
    And the created booking should match the request

    Examples:
      | deposit | needs            |
      | true    | Breakfast        |
      | false   | Airport transfer |
      | true    | Late checkout    |

  # Known API defects: the scenarios below document the CURRENT behaviour.
  # They pass today and will fail (on purpose) once the API is fixed,
  # which is the signal to update the expected result.

  @negative @known-defect
  Scenario Outline: Creating a booking without a required field - <field>
    Given a booking payload without the "<field>" field
    When I create the booking
    # Expected: 400 Bad Request with a validation message. Actual: 500.
    Then the response status code should be 500

    Examples:
      | field        |
      | firstname    |
      | lastname     |
      | bookingdates |

  @negative @known-defect
  Scenario: A booking with checkout before checkin is accepted
    Given a valid booking with checkout before checkin
    When I create the booking
    # Expected: 400 Bad Request. Actual: the API stores an impossible stay.
    Then the response status code should be 200
