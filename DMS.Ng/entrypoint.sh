#!/bin/sh
set -eu

find /usr/share/nginx/html -type f -exec sed -i "s|\${API_URL}|${API_URL:-}|g" {} +
find /usr/share/nginx/html -type f -exec sed -i "s|\${GITHUB_SHA}|${GITHUB_SHA:-}|g" {} +

nginx -g 'daemon off;'
