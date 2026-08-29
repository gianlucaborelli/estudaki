<script lang="ts">
	import { onMount } from 'svelte';
	import { Button } from 'flowbite-svelte';
	import { SunOutline, MoonOutline } from 'flowbite-svelte-icons';

	let dark = $state(false);

	onMount(() => {
		dark = localStorage.getItem('theme') === 'dark';
		document.documentElement.classList.toggle('dark', dark);
	});

	function toggleTheme() {
		dark = !dark;

		document.documentElement.classList.toggle('dark', dark);

		localStorage.setItem('theme', dark ? 'dark' : 'light');
	}
</script>

<Button class="theme-toggle"
	onclick={toggleTheme}
	aria-label={dark ? 'Ativar tema claro' : 'Ativar tema escuro'}
>
	{#if dark}
		<SunOutline />
	{:else}
		<MoonOutline />
	{/if}
</Button>

<style>
	:global(.theme-toggle) {
		color: var(--text);
	}

	:global(.theme-toggle:focus) {
		outline: none;
		box-shadow: none;
	}

	:global(.theme-toggle:focus-visible) {
		outline: none;
		box-shadow: none;
	}

	:global(.theme-toggle:focus-visible) {
		--tw-ring-shadow: 0 0 transparent;
	}

</style>