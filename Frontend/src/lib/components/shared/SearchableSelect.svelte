<script lang="ts" generics="T">
	import type { SelectOptionType } from 'flowbite-svelte';

	type Props = {
		items: SelectOptionType<T>[];
		value?: T[];
		label?: string;
		placeholder?: string;
		searchPlaceholder?: string;
		class?: string;
	};

	let {
		items,
		value = $bindable<T[]>([]),
		label,
		placeholder = 'Selecione uma opção',
		searchPlaceholder = 'Pesquisar...',
		class: className = ''
	}: Props = $props();

	const selectId = $props.id();
	let isOpen = $state(false);
	let search = $state('');

	let filteredItems = $derived(
		items.filter((item) => String(item.name).toLowerCase().includes(search.toLowerCase()))
	);

	let selectedItems = $derived(items.filter((item) => value.includes(item.value)));

	function toggleOpen() {
		isOpen = !isOpen;

		if (!isOpen) {
			search = '';
		}
	}

	function toggleItem(item: SelectOptionType<T>) {
		if (item.disabled) return;

		value = value.includes(item.value)
			? value.filter((selected) => selected !== item.value)
			: [...value, item.value];
	}

	function close() {
		isOpen = false;
		search = '';
	}
</script>

<svelte:window onclick={close} />

<div class={`searchable-select-container ${className}`}>
	{#if label}
		<span id={`${selectId}-label`} class="searchable-select-label">{label}</span>
	{/if}

	<div class="searchable-select" role="presentation" onclick={(event) => event.stopPropagation()}>
		<button
			type="button"
			class="searchable-select-trigger"
			aria-haspopup="listbox"
			aria-expanded={isOpen}
			aria-labelledby={label ? `${selectId}-label` : undefined}
			onclick={toggleOpen}
		>
			{#if selectedItems.length}
				<span class="selected-count">
					{selectedItems.length}
					{selectedItems.length === 1 ? ' selecionado' : ' selecionados'}
				</span>
			{:else}
				<span class="searchable-select-value placeholder">{placeholder}</span>
			{/if}

			<svg
				class="searchable-select-chevron"
				class:rotate-180={isOpen}
				viewBox="0 0 20 20"
				fill="currentColor"
			>
				<path
					fill-rule="evenodd"
					d="M5.23 7.21a.75.75 0 0 1 1.06.02L10 11.168l3.71-3.937a.75.75 0 1 1 1.09 1.03l-4.25 4.5a.75.75 0 0 1-1.09 0l-4.25-4.5a.75.75 0 0 1 .02-1.06Z"
					clip-rule="evenodd"
				/>
			</svg>
		</button>

		{#if isOpen}
			<div class="searchable-select-dropdown" role="listbox" aria-multiselectable="true">
				<input
					type="text"
					bind:value={search}
					placeholder={searchPlaceholder}
					class="searchable-select-search"
					onclick={(event) => event.stopPropagation()}
				/>

				<div class="searchable-select-list">
					{#each filteredItems as item (String(item.value))}
						{@const isSelected = value.includes(item.value)}
						<button
							type="button"
							role="option"
							aria-selected={isSelected}
							class="searchable-select-item"
							class:selected={isSelected}
							disabled={item.disabled}
							onclick={() => toggleItem(item)}
						>
							<span class="searchable-select-checkbox" class:checked={isSelected} aria-hidden="true"
							></span>
							{item.name}
						</button>
					{:else}
						<div class="searchable-select-empty">Nenhum resultado encontrado</div>
					{/each}
				</div>
			</div>
		{/if}
	</div>
</div>

<style>
	.searchable-select-container {
		display: grid;
		gap: 0.45rem;
		min-width: 13rem;
		color: var(--text);
		font-size: 0.875rem;
		font-weight: 600;
		line-height: 1.25;
	}

	.searchable-select {
		position: relative;
	}

	.searchable-select-label {
		color: var(--text);
		font-size: 0.875rem;
		font-weight: 600;
		line-height: 1.25;
	}

	.searchable-select-trigger {
		display: flex;
		width: 100%;
		align-items: center;
		justify-content: space-between;
		gap: 0.5rem;
		min-height: 2.75rem;
		border: 1px solid var(--border);
		border-radius: 0.5rem;
		background-color: var(--surface-elevated);
		box-shadow: 0 1px 2px rgb(0 0 0 / 0.05);
		color: var(--text);
		cursor: pointer;
		font-size: 0.875rem;
		padding: 0 0.75rem;
		text-align: left;
		transition:
			border-color 0.2s ease,
			box-shadow 0.2s ease,
			background-color 0.2s ease;
	}

	.searchable-select-trigger:hover {
		border-color: var(--secondary-hover);
	}

	.searchable-select-trigger:focus-visible {
		border-color: var(--secondary);
		outline: none;
		box-shadow: 0 0 0 3px color-mix(in srgb, var(--secondary) 24%, transparent);
	}

	.searchable-select-value {
		overflow: hidden;
		min-width: 0;
		font-size: 0.875rem;
		text-overflow: ellipsis;
		white-space: nowrap;
	}

	.searchable-select-value.placeholder {
		color: var(--text-muted);
		font-weight: 400;
	}

	.searchable-select-chevron {
		flex: 0 0 auto;
		width: 1rem;
		height: 1rem;
		color: var(--secondary);
		transition: transform 0.2s ease;
	}

	.searchable-select-chevron.rotate-180 {
		transform: rotate(180deg);
	}

	.searchable-select-dropdown {
		position: absolute;
		z-index: 50;
		margin-top: 0.35rem;
		width: 100%;
		overflow: hidden;
		border: 1px solid var(--border);
		border-radius: 0.5rem;
		background-color: var(--surface-elevated);
		box-shadow: 0 12px 24px rgb(0 0 0 / 0.14);
		padding: 0.5rem;
	}

	.searchable-select-search {
		width: 100%;
		min-height: 2.25rem;
		margin-bottom: 0.5rem;
		border: 1px solid var(--border);
		border-radius: 0.375rem;
		background-color: var(--surface-elevated);
		padding: 0 0.75rem;
		color: var(--text);
		font-size: 0.8125rem;
		outline: none;
		transition:
			border-color 0.2s ease,
			box-shadow 0.2s ease;
	}

	.searchable-select-search:hover {
		border-color: var(--secondary-hover);
	}

	.searchable-select-search:focus-visible {
		border-color: var(--secondary);
		box-shadow: 0 0 0 3px color-mix(in srgb, var(--secondary) 24%, transparent);
	}

	.searchable-select-list {
		display: grid;
		gap: 0.15rem;
		max-height: 15rem;
		overflow-y: auto;
	}

	.searchable-select-item {
		display: flex;
		align-items: center;
		gap: 0.5rem;
		width: 100%;
		border: 0;
		border-radius: 0.375rem;
		background-color: transparent;
		color: var(--text);
		cursor: pointer;
		font-size: 0.875rem;
		padding: 0.5rem 0.75rem;
		text-align: left;
	}

	.searchable-select-item:hover {
		background-color: color-mix(in srgb, var(--secondary) 12%, var(--surface-elevated));
		color: var(--text);
	}

	.searchable-select-item.selected {
		background-color: color-mix(in srgb, var(--secondary) 18%, var(--surface-elevated));
		color: var(--secondary-active);
		font-weight: 600;
	}

	.searchable-select-item:disabled {
		cursor: not-allowed;
		opacity: 0.5;
	}

	.searchable-select-checkbox {
		flex: 0 0 auto;
		width: 1rem;
		height: 1rem;
		border: 1px solid var(--secondary);
		border-radius: 0.25rem;
		background-color: transparent;
		transition:
			background-color 0.15s ease,
			border-color 0.15s ease;
	}

	.searchable-select-checkbox.checked {
		border-color: var(--secondary);
		background-color: var(--secondary);
	}

	.searchable-select-empty {
		padding: 0.5rem 0.75rem;
		color: var(--text-muted);
		font-size: 0.875rem;
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
