# Contributing

Thanks for contributing.

## Development Setup

1. Follow setup instructions in [README.md](README.md).
2. Start backend and frontend locally.
3. Verify builds before opening a pull request.

## Pull Request Checklist

- Keep changes focused and small
- Add or update tests when behavior changes
- Keep naming/style consistent with surrounding code
- Ensure both apps build successfully:

```bash
cd backend && dotnet build
cd ../frontend && npm run build
```

## Reporting Issues

Please include:

- Steps to reproduce
- Expected vs actual behavior
- Screenshots or logs when relevant
- Environment details (OS, .NET SDK, Node version)
