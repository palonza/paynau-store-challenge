#!/bin/bash
echo "Building Docker image..."
docker build -t paynau-backend:latest .
echo "Docker image built successfully!"