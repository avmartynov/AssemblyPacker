"Cleaning project $(Resolve-Path .\)."
del -ErrorAction SilentlyContinue -Recurse bin
del -ErrorAction SilentlyContinue -Recurse obj
del -ErrorAction SilentlyContinue *.user
