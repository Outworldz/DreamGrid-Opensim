#!/bin/sh
echo "Scrubbing Directories"
find . -name "*.csproj" -type f -print0 | xargs -0 /bin/rm -f
find . -name "*.csproj.user" -type f -print0 | xargs -0 /bin/rm -f
find . -name "*.build" -type f -print0 | xargs -0 /bin/rm -f
find . -name "*Temporary*" -type f -print0 | xargs -0 /bin/rm -f
find . -name "*.cache" -type f -print0 | xargs -0 /bin/rm -f
find . -name "*.rej" -type f -print0 | xargs -0 /bin/rm -f
find . -name "*.orig" -type f -print0 | xargs -0 /bin/rm -f
find . -name "*.pdb" -type f -print0 | xargs -0 /bin/rm -f
find . -name "*.mdb" -type f -print0 | xargs -0 /bin/rm -f
find . -name "*obj" -type f -print0 | xargs -0 /bin/rm -rf
find . -name "obj" -type d -print0 | xargs -0 /bin/rm -rf
find . -name "*.swp" -type f -print0 | xargs -0 /bin/rm -f