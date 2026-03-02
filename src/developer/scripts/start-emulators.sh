#!/bin/bash

# this is in a script file because escaping this stuff in package.json is a pain
firebase emulators:start --import ../../data/firebase --only auth,firestore,functions "$@" 2> >(grep -Ev 'lsof|Output information may be incomplete|assuming "dev=.*" from mount table' >&2)
