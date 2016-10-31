# funcswipl
Functional interface to SWI-Prolog, written in F#

The goal of this project is to make the SWI-Prolog runtime available to F# applications in an idiomatic way.  This is very much a work in progress, so use with caution.

A few important notes:

* Adjust the path to libswipl to a valid path on your system.
* Set the `SWI_HOME_DIR` environment variable to the root of your SWI-Prolog installation before trying to run anything.
* If running on Mono, you'll need to build the runtime for 64-bit, because the Linux and OS X SWI-Prolog is 64-bit these days.
* Thread safety is non-existent. Not sure exactly how that works in the native library, and until that is resolved, do not use in a multithreaded environment.

PR's and issues welcome!
