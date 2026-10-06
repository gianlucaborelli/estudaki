import tailwindcss from '@tailwindcss/vite';
import adapter from '@sveltejs/adapter-node';
import { sveltekit } from '@sveltejs/kit/vite';
import { defineConfig } from 'vite';

export default defineConfig({
	plugins: [
		tailwindcss(),
		sveltekit({
			compilerOptions: {
				// Force runes mode for the project, except for libraries. Can be removed in svelte 6.
				runes: ({ filename }) => filename.split(/[/\\]/).includes('node_modules') ? undefined : true
			},

			// adapter-node: standalone Node server, used to run the frontend inside the Docker image.
			adapter: adapter(),

			// Behind the reverse proxy, the Host/X-Forwarded-* headers aren't reliable enough for
			// SvelteKit to resolve the correct origin, so legitimate origins are allow-listed explicitly
			// instead of relying on header-based origin detection for the CSRF check.
			csrf: {
				trustedOrigins: ['https://estudaki.com.br', 'https://www.estudaki.com.br']
			}
		})
	]
});
