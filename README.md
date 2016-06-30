# DMW Suite
The Digital Model Watermarking Suite (Hence DMW Suite or just DMW) is a system for encrypting and watermarking models, as well as providing various security assurances such as detecting modified models.

## Required Dependencies
* OpenTK: http://www.opentk.com/

## Current Features
* Embed data within model vertex data, then read later.
* Plugin support (Custom model importers and exporters).
 * OBJ Model Support
* Model similarity detector.
* Model hash checking - detects model changes.
* Custom extensible binary model format, with support for encryption.
 * Built in support for Rijndael encryption seeded with a password.

## Upcoming Features
* A remote webserver using the model similarity data, such that modified files can retain their watermark.
 * Remote server could also log file movement, detecting when DMW files have been opened.
* Additional watermarking methods - Mix and match!
* Additional encryption schemes.
* Additional file importers and exporters.
