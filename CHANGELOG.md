# Changelog

All notable changes to this project will be documented in this file.

## [Unreleased]

### Security
- Removed hardcoded JWT signing key from source code; now reads from configuration at runtime.

### Added
- Added functionality to resend email with CFDI XML and PDF attachments.
- Added functionality to retrieve CFDI by UUID.
- Implemented QuestPDF for CFDI PDF layout and PDF persistence.

### Changed
- Added functionality to generate or regenerate a new CFDI PDF version.
- Added comprehensive README and AGENTS.md documentation.

## [1.0.0] - 2026-05-31

### Added
- Initial release.
