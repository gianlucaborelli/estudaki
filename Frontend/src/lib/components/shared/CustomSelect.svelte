<script lang="ts" generics="T">
	import { Label, MultiSelect, type SelectOptionType } from 'flowbite-svelte';

	type Props = {
		items: SelectOptionType<T>[];
		value?: T[];
		label?: string;
		placeholder?: string;
	};

	let { items, value = $bindable<T[]>([]), label, placeholder = 'Selecione...' }: Props = $props();
	const selectId = $props.id();
</script>

<div class="select-container">
	<Label for={selectId} class="select-label">
		{#if label}
			{label}
		{/if}
		<MultiSelect
			id={selectId}
			{items}
			bind:value
			{placeholder}
			class="select mt-1"
			classes={{ dropdown: 'select-dropdown' }}
		>
			{#snippet children({ item })}
				{#if value.length > 0 && item.value === value[0]}
					<div class="flex items-center gap-1">
						<span class="selected-count">
							{value.length}
							{value.length === 1 ? ' selecionado' : ' selecionados'}
						</span>
					</div>
				{/if}
			{/snippet}
		</MultiSelect>
	</Label>
</div>

<style>
	.select-container {
		min-width: 13rem;
	}

	:global(.select-label) {
		display: grid;
		gap: 0.45rem;
		color: var(--text);
		font-size: 0.875rem;
		font-weight: 600;
		line-height: 1.25;
	}

	:global(.select) {
		min-height: 2.75rem;
		border: 1px solid var(--border);
		border-radius: 0.5rem;
		background-color: var(--surface-elevated);
		color: var(--text);
		box-shadow: 0 1px 2px rgb(0 0 0 / 0.05);
		font-size: 0.875rem;
		transition:
			border-color 0.2s ease,
			box-shadow 0.2s ease,
			background-color 0.2s ease;
	}

	:global(.select:hover) {
		border-color: var(--secondary-hover);
	}

	:global(.select:focus-visible) {
		border-color: var(--secondary);
		outline: none;
		box-shadow: 0 0 0 3px color-mix(in srgb, var(--secondary) 24%, transparent);
	}

	:global(.select > span) {
		color: var(--text);
		font-size: 0.875rem;
	}

	:global(.select > span:first-of-type) {
		color: var(--text-muted);
	}

	:global(.select svg) {
		color: var(--secondary);
	}

	:global(.select-dropdown) {
		margin-top: 0.35rem;
		overflow: hidden;
		border: 1px solid var(--border);
		border-radius: 0.5rem;
		background-color: var(--surface-elevated);
		box-shadow: 0 12px 24px rgb(0 0 0 / 0.14);
	}

	:global(.select-dropdown > div) {
		color: var(--text);
		font-size: 0.875rem;
	}

	:global(.select-dropdown > div:hover),
	:global(.select-dropdown > div[data-active='true']) {
		background-color: color-mix(in srgb, var(--secondary) 12%, var(--surface-elevated));
		color: var(--text);
	}

	:global(.select-dropdown > div[data-selected='true']) {
		background-color: color-mix(in srgb, var(--secondary) 18%, var(--surface-elevated));
		color: var(--secondary-active);
		font-weight: 600;
	}

	.selected-count {
		display: inline-flex;
		align-items: center;
		border-radius: 999px;
		background-color: color-mix(in srgb, var(--secondary) 14%, transparent);
		color: var(--secondary-active);
		font-size: 0.75rem;
		font-weight: 700;
		line-height: 1;
		padding: 0.3rem 0.5rem;
	}
</style>
