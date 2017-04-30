# The Alan & Albus Adventures - Coding rules

We will be conforming with the [Unity C# Coding Guidelines](http://wiki.unity3d.com/index.php/Csharp_Coding_Guidelines)

In addition the following applies:

Naming conventions:

 * All .cs (C#) files use PascalCase.
 * All class names in code use PascalCase.
 * Private variables use camelCase.
 * Constant/Readonly variables use UPPERCASE, this overrides other rules.
 * Method parameters always use camelCase.
 * Method names use PascalCase.
 * Always write ID with uppercase, this overwrites the other rules.
    * Do not write Id or id.
 
Class structure, in this order:
 * Nested classes
 * Public members
 * Private members
 * Constructor (if applicable)
 * Public methods
 * Unity specific methods (such as Start, Awake, Update, etc...)
 * Private methods

[Allman style](https://en.wikipedia.org/wiki/Indent_style#Allman_style) is used for all brackets in .cs files.

Brackets are mandatory for statement blocks (for, if, while, etc...).

"case" statements should be indented inside a switch block.

Never ommit the "public" or "private" keywords from method or variable declaration.

Have 1 line between methods.

Have 1 space between operators and variables/values.

Have 1 space between slashes and the beginning of a comment (i.e. "// some comment").

Maximum file length is 400 lines.

Maximum line length is 100 characters.

Keep in mind KISS (Keep It Simple Stupid) and SRP (Single Responsibility Principle).
