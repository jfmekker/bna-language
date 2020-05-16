### bna-language

![Logo](https://github.com/jfmekker/bna-language/raw/master/logo.png)

# BNA's Not Assembly
BNA is an assembly-inspired programming language designed to keep things strict and simple. Operations are atomic, and the syntax is *close* to English. See the wiki for more information.

## Pronunciation
BNA is meant to be pronounced as 'banana', but feel free to call it 'buh-nah' or just 'BNA'.
The acronym takes inspiration from other recursive acronyms, primarily GNU (GNU's Not Unix).

## Contributing
BNA is still in an early beta stage, so it will be difficult to have parallel development without running into conflicts. That being said, contributions are very welcome. Check the issues list for work to pickup, or open issues for bugs, feature requests, or wiki updates.

## Future Plans
BNA currently translates BNA code to equivalent Python. However, this is somewhat limiting when it comes to type-enforcement and dynamic jumps. The next few minor releases will see more functionality added to this, as well as cleaning things up for a full semi-stable release.

In the (hopefully near) future, verison `v1.0-beta` will be released which will no longer support translation to Python. Instead, BNA will be compiled directly to our own custom bytecode/binary format. This can then be run through a BNA runtime virtual environment similiar to the Java virtual machine. In future releases, we may support translation to other langauges like C, C++, C#, etc.

### Roadmap

Versions supporting BNA translation to Python:

 - `v0.1-alpha` Functioning proof of concept for the BNA compiler.
 - `v0.2-alpha` Basic control flow and bit operations.
 - `v0.3-alpha` Math functions and fixes.
 - `v0.4-alpha` Strings.
 - `v0.5-beta`  Lists.
 - `v0.6-beta`  **PLANNED** Command line and file input and output.
 - `v0.7-beta`  **PLANNED** Improved language specification based on pedagogical feedback.
 - `v0.8`       **PLANNED** Included education materials.

 Tentaive future versioning:

  - `v1.0-beta`  **PLANNED** BNA virtual environment.
  - `v1.1-beta`  **PLANNED** Dynamic jumps and scopes.
