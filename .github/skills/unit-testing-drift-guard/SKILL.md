---
name: unit-testing-drift-guard
description: 'Design and implement robust unit tests that verify behavioral outcomes, not surface smell checks. Use for scenario coverage, deterministic assertions, and immediate drift detection across behavior changes.'
argument-hint: 'Feature, action, or behavior to verify'
user-invocable: true
disable-model-invocation: false
---

# Unit Testing Drift Guard

## What This Skill Produces

This skill creates or updates unit tests that:

- verify the result of an action against explicit behavior contracts
- cover consistent behavior across meaningful scenarios, including edge cases
- detect behavioral drift immediately when implementation changes

## When to Use

Use this skill when you need tests for:

- new features with defined expected behavior
- regressions where behavior changed unexpectedly
- refactors that must preserve behavior
- bug fixes that require prevention of recurrence

Do not use this skill for superficial checks that only assert non-null, render presence, or other weak smell-test signals unless those are part of a broader behavioral contract.

## Required Inputs

1. Behavior under test: what action happens and what must be true after it.
2. Invariants: what must stay consistent across all supported scenarios.
3. Scenario matrix: normal, boundary, invalid, and interaction cases.

## Procedure

1. Define the contract first.
   Document preconditions, action, observable outputs, and invariants.

2. Build a scenario matrix.
   The matrix must contain a minimum of 5 scenarios:
   - happy path
   - boundary values
   - invalid input or failure mode
   - state transition or sequencing case
   - one regression case if there is known history
   - additional scenarios for any distinct behavioral invariant that is not yet covered
   If fewer than 5 distinct scenarios can be identified, treat that as a signal that the behavior contract is under-specified. Stop and request clarification before proceeding.

3. Choose high-signal assertions.
   Prefer exact observable outcomes over broad existence checks.
   Examples of strong assertions:
   - returned value or output shape
   - emitted event payload
   - state transition and side effects
   - idempotence or ordering guarantees

4. Design drift-detection checks.
   Every tested behavior must have at least one parameterized invariant test.
   Drift-detection is not optional or restricted to high-risk behaviors.
   Typical drift checks:
   - scenario parameterization that asserts the same invariant across varied inputs
   - explicit baseline expectation for business rules
   - repeated-call consistency when determinism is required
   A test suite that has no invariant test for a behavior is incomplete regardless of how many scenario tests exist.

5. Isolate external variability.
   Non-deterministic behavior is a code smell by default and must be examined before tests are written.
   Decision sequence:
   a. Identify any source of non-determinism in the code under test (clock, random, I/O, concurrency, external service).
   b. Treat its presence as a smell and escalate for refactoring unless it is an explicit and justified design decision.
   c. If the non-determinism cannot be removed or is justified (for example: async scheduling, clipboard interaction), introduce a deterministic seam and mock the non-deterministic dependency.
   d. The justification for keeping the non-determinism must be documented in the test fixture with a comment.
   Never write tests that tolerate or ignore non-determinism silently.

6. Implement tests with clear naming.
   Name format:
   - Action_WhenCondition_ThenExpectedOutcome
   Keep one behavioral claim per test where practical.

7. Validate completeness before finishing.
   Confirm every contract clause is covered by at least one test.

## Decision Points

1. If behavior is ambiguous:
   Stop and request explicit expected outcomes before writing tests.

2. If behavior depends on environment or time:
   First evaluate whether the non-determinism is a design smell and whether it can be refactored out.
   If it cannot, introduce a deterministic seam and mock the dependency with documented justification.

3. If a single test hides multiple claims:
   Split into focused tests to improve drift localization.

4. If failures are hard to diagnose:
   Strengthen assertion messages and narrow fixture setup.

## Quality Gates

A test set is considered complete only when all gates pass:

1. Outcome Gate:
   Every test verifies concrete post-action behavior.

2. Scenario Gate:
   The matrix includes at least happy path, boundary, and failure/invalid paths.

3. Invariant Gate:
   Every tested behavior has at least one parameterized invariant test.
   No exceptions: if a behavior has no invariant test, the suite is incomplete.

4. Drift Gate:
   At least one test would fail on semantic drift even if public signatures remain unchanged.

5. Signal Gate:
   Assertions are specific enough to point directly to the broken rule.

## Completion Checklist

1. Behavior contract documented.
2. Scenario matrix implemented.
3. Weak smell-only assertions removed or upgraded.
4. Drift-detection tests included.
5. Test names reflect action, condition, and outcome.
6. All non-determinism examined, refactored out where possible, mocked with justification where not.
7. Minimum 5 scenarios per behavior contract.

## Example Prompts

- Create unit tests for markdown code block copy behavior using this drift-guard workflow.
- Add tests for theme resource merge behavior and include boundary and drift checks.
- Refactor existing tests to remove smell checks and enforce outcome-based assertions.

## Related Next Customizations

1. Add a companion prompt that generates a scenario matrix from a behavior description.
2. Add repository-specific assertion conventions for naming and failure messages.
3. Add a lightweight review checklist instruction for rejecting low-signal tests in PRs.
