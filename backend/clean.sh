#!/bin/bash
echo "Cleaning build artifacts..."
dotnet clean
rm -rf **/bin
rm -rf **/obj
echo "Clean completed!"