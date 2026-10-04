#!/bin/bash
set -e

dotnet /app/backend/Estudaki.Api.dll &
BACKEND_PID=$!

PORT=3000 HOST=0.0.0.0 node /app/frontend/build/index.js &
FRONTEND_PID=$!

PORT=4000 node /app/backoffice/server/server.mjs &
BACKOFFICE_PID=$!

shutdown() {
    kill -TERM "$BACKEND_PID" "$FRONTEND_PID" "$BACKOFFICE_PID" 2>/dev/null
}
trap shutdown TERM INT

# Exit as soon as any process dies, so the container restarts and all three come back up together.
wait -n "$BACKEND_PID" "$FRONTEND_PID" "$BACKOFFICE_PID"
EXIT_CODE=$?

shutdown
wait
exit $EXIT_CODE
