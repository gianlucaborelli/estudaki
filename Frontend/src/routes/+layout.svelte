<script lang="ts">
	import './layout.css';
	import '@fontsource-variable/manrope/wght.css';	
	import '@fontsource-variable/inter/wght.css';
	import favicon from '$lib/assets/favicon.ico';
	import AppBar from '$lib/components/layout/AppBar.svelte';
	import Footer from '$lib/components/layout/Footer.svelte';
	
	let { children } = $props();
</script>

<svelte:head>
	<link rel="icon" href={favicon} />
</svelte:head>

<div class="app-layout">
	<AppBar />

	<main class="page-container">
		{@render children()}
	</main>

	<Footer />
</div>
