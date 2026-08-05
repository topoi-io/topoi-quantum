# Security Policy

Topoi takes the security, correctness and integrity of Topoi Quantum seriously.

This policy explains which versions receive security updates, how to report a suspected vulnerability and what reporters can expect after submitting a report.

## Supported Versions

Topoi Quantum is currently in its initial release-candidate stage.

| Version                                  | Security support                                                        |
| ---------------------------------------- | ----------------------------------------------------------------------- |
| Latest `0.1.0` release candidate         | Supported                                                               |
| Older `0.1.0` release candidates         | Supported only until users have had a reasonable opportunity to upgrade |
| Development branches and untagged builds | Best effort                                                             |
| Versions older than `0.1.0`              | Not supported                                                           |

Users should upgrade to the latest available version before reporting a problem that may already have been corrected.

Once stable releases are available, this table will be updated to identify the specific release lines that continue to receive security fixes.

## Reporting a Vulnerability

Please do not report suspected security vulnerabilities through a public GitHub issue, public discussion, pull request or social-media post.

Use GitHub's private vulnerability-reporting system:

1. Open the Topoi Quantum repository on GitHub.
2. Select the **Security** tab.
3. Select **Advisories** or **Report a vulnerability**.
4. Create a private vulnerability report containing the available technical details.

If the private reporting option is unavailable, contact a repository administrator privately through GitHub and request a secure reporting channel.

Do not include sensitive vulnerability details in a public message.

## Information to Include

A useful report should include as much of the following information as possible:

* The affected Topoi Quantum package and version.
* The affected operating system and .NET SDK/runtime version.
* A description of the vulnerability.
* The potential security impact.
* The conditions required to reproduce it.
* Clear reproduction steps.
* A minimal proof of concept, when safe to provide.
* Relevant log output or exception details.
* Whether the problem affects the SDK, OpenQASM parser, command-line tool, package installation or another component.
* Any suggested mitigation or correction.
* Whether the vulnerability is already publicly known.

Please remove API keys, access tokens, private source code, personal information and other unrelated secrets from submitted evidence.

## What to Expect

After receiving a report, the Topoi maintainers aim to:

* Acknowledge the report within three business days.
* Perform an initial assessment within seven business days.
* Confirm whether the issue is accepted, rejected, duplicated or requires more information.
* Provide periodic status updates while an accepted vulnerability is being investigated.
* Coordinate disclosure and release timing with the reporter where practical.
* Credit the reporter in the advisory or release notes when requested and appropriate.

These are response targets rather than contractual guarantees. Complex vulnerabilities may require additional investigation and remediation time.

## Disclosure Process

For an accepted vulnerability, the maintainers will normally:

1. Reproduce and assess the issue.
2. Determine the affected versions and components.
3. Develop a correction or mitigation.
4. Add regression tests where appropriate.
5. Prepare patched NuGet packages or tool releases.
6. Publish a GitHub Security Advisory when appropriate.
7. Document the correction in `CHANGELOG.md`.
8. Coordinate public disclosure after users have access to a corrected version.

Please allow the maintainers a reasonable opportunity to investigate and correct an accepted vulnerability before publishing technical details.

## Security Scope

Security reports may include vulnerabilities affecting:

* Official Topoi Quantum NuGet packages.
* The official `Topoi.Quantum.Tool` .NET global tool.
* Package installation or dependency resolution.
* OpenQASM parsing and execution.
* Native circuit and command parsing.
* Malicious or malformed input handling.
* File access initiated by the command-line tool.
* Resource-exhaustion issues that can be triggered unexpectedly or remotely.
* Exposure of sensitive information.
* Unsafe code execution.
* Package-supply-chain integrity.
* Security-sensitive randomness or measurement behaviour.
* GitHub Actions and official release automation.

Correctness defects that could cause users to make security-sensitive decisions may also be reported privately.

## Out of Scope

The following are generally not treated as security vulnerabilities unless they create a concrete security impact:

* Feature requests.
* Unsupported OpenQASM syntax.
* Documented simulator limitations.
* Normal exponential memory requirements of dense state-vector simulation.
* Performance differences without a security or availability impact.
* Problems affecting unsupported versions only.
* Vulnerabilities in third-party software that do not affect Topoi Quantum.
* Social-engineering attacks against Topoi employees or contributors.
* Physical attacks against user devices.
* Denial-of-service testing that risks disrupting systems belonging to others.

Normal bugs and correctness issues may be reported through GitHub Issues when they do not contain sensitive security information.

## Safe-Harbour Statement

Topoi supports good-faith security research conducted responsibly.

The maintainers will not pursue action against researchers who:

* Make a good-faith effort to comply with this policy.
* Avoid accessing, modifying or deleting data belonging to others.
* Avoid privacy violations and service disruption.
* Test only systems and software they are authorised to test.
* Report vulnerabilities privately.
* Provide reasonable time for investigation and remediation.
* Do not exploit a vulnerability beyond what is required to demonstrate it.

This safe-harbour statement does not authorise testing of third-party infrastructure, NuGet, GitHub, Microsoft services or systems not owned or controlled by Topoi.

## Package Integrity

Users should install Topoi Quantum packages only from official sources identified by the Topoi project.

Before installing a package, users should verify:

* The package ID.
* The package version.
* The listed author or owner.
* The repository metadata.
* The package source.
* Any available package signature or provenance information.

Unexpected packages using similar names should be treated with caution and reported privately to the maintainers.

## Security Updates

Security-related corrections will be documented through one or more of the following:

* GitHub Security Advisories.
* GitHub Releases.
* `CHANGELOG.md`.
* Updated NuGet packages.
* Repository announcements.

Security releases may contain limited technical information until users have had a reasonable opportunity to upgrade.
