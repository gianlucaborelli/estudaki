import 'dotenv/config';

import {
  AngularNodeAppEngine,
  createNodeRequestHandler,
  isMainModule,
  writeResponseToNodeResponse,
} from '@angular/ssr/node';
import express from 'express';
import { join } from 'node:path';

const browserDistFolder = join(import.meta.dirname, '../browser');

const app = express();
const angularApp = new AngularNodeAppEngine();


const apiUrl = process.env['API_URL'];

app.use('/api', async (req, res) => {
  try {
    const targetUrl = `${apiUrl}${req.originalUrl}`;
    console.log(`Proxying request to: ${targetUrl}`);
    const headers = new Headers();

    // Encaminha headers relevantes
    for (const [key, value] of Object.entries(req.headers)) {
      if (value !== undefined && key.toLowerCase() !== 'host') {
        if (Array.isArray(value)) {
          value.forEach(v => headers.append(key, v));
        } else {
          headers.set(key, value);
        }
      }
    }

    const hasBody = !['GET', 'HEAD'].includes(req.method);

    const response = await fetch(targetUrl, {
      method: req.method,
      headers,
      // Node's fetch requires streamed request bodies to be typed as `any` and declare `duplex`
      body: (hasBody ? req : undefined) as any,
      duplex: hasBody ? 'half' : undefined,
    } as RequestInit);

    // Status
    res.status(response.status);

    // Headers da resposta
    response.headers.forEach((value, key) => {
      // Set-Cookie precisa de tratamento especial
      if (key.toLowerCase() === 'set-cookie') {
        return;
      }

      res.setHeader(key, value);
    });

    // Repassa cookies
    const setCookie = response.headers.getSetCookie();

    if (setCookie.length > 0) {
      res.setHeader('Set-Cookie', setCookie);
    }

    const body = await response.arrayBuffer();

    res.send(Buffer.from(body));

  } catch (error) {
    console.error(error);

    res.status(502).json({
      message: 'Erro ao comunicar com a API',
    });
  }
});
/**
 * Example Express Rest API endpoints can be defined here.
 * Uncomment and define endpoints as necessary.
 *
 * Example:
 * ```ts
 * app.get('/api/{*splat}', (req, res) => {
 *   // Handle API request
 * });
 * ```
 */

/**
 * Serve static files from /browser
 */
app.use(
  express.static(browserDistFolder, {
    maxAge: '1y',
    index: false,
    redirect: false,
  }),
);

/**
 * Handle all other requests by rendering the Angular application.
 */
app.use((req, res, next) => {
  angularApp
    .handle(req)
    .then((response) =>
      response ? writeResponseToNodeResponse(response, res) : next(),
    )
    .catch(next);
});

/**
 * Start the server if this module is the main entry point, or it is ran via PM2.
 * The server listens on the port defined by the `PORT` environment variable, or defaults to 4000.
 */
if (isMainModule(import.meta.url) || process.env['pm_id']) {
  const port = process.env['PORT'] || 4000;
  app.listen(port, (error) => {
    if (error) {
      throw error;
    }

    console.log(`Node Express server listening on http://localhost:${port}`);
  });
}

/**
 * Request handler used by the Angular CLI (for dev-server and during build) or Firebase Cloud Functions.
 */
export const reqHandler = createNodeRequestHandler(app);
