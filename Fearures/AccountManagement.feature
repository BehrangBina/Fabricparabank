Feature: Customer Onboarding and Banking Operations
As a new user
I want to register, log in, and perform basic banking tasks
So that I can use the core features of the Para Bank application

Scenario: End-to-end banking workflow for a newly registered user
    Given I navigate to the Para Bank application
    When I register a new user with a unique username
    
#    And I log in using the newly registered user credentials
#    Then I should see the global navigation menu available and functional
#
#    When I create a new Savings account
#    Then I should see the account listed in the Accounts Overview page with correct balance
#
#    When I transfer funds from the new account to another account
#    Then I should see the updated balance reflected accordingly
#
#    When I pay a bill using the new account
#    Then the payment should be processed and balance updated