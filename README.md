### bna-language

![Logo](https://github.com/jfmekker/bna-language/raw/master/logo.png)

# BNA's Not Assembly
BNA is an assembly-inspired programming language designed to keep things strict and simple. Operations are atomic, and the syntax is *close* to English. See the wiki for more information.

## Pronunciation
BNA  is meant to be pronounced as 'banana', but feel free to call it 'buh-nah' or just 'BNA'.
The acronym takes inspiration from other recursive acronyms, primarily GNU (GNU's Not Unix).

## Contributing
BNA is in **very** early stages, so direct *code* contributions most likely won't be taken. However, please offer any advice or suggestions!

## Future Plans
BNA currently translates BNA code to equivalent Python. However, this is somewhat limiting when it comes to type-enforcement and dynamic jumps. The next minor release or two will see more functionality added to this, as well as cleaning things up for a full semi-stable release.

In the (hopefully near) future, verison `v1.0-beta` will be released which will no longer support translation to Python. Instead, BNA will be compiled directly to our own custom bytecode/binary format. This can then be run through a BNA runtime virtual environment similiar to the Java virtual machine. In future releases, we may support translation to other langauge like C, C++, C#, etc.