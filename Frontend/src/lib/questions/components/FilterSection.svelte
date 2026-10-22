<script lang="ts">
	import CustomSelect from '$lib/components/shared/CustomSelect.svelte';
	import SearchableSelect from '$lib/components/shared/SearchableSelect.svelte';
	import { Badge, CloseButton, type SelectOptionType } from 'flowbite-svelte';
	import { ChevronDownOutline } from 'flowbite-svelte-icons';

	type SelectedFilter = {
		label: string;
		items: { key: string; name: string; remove: (event: MouseEvent) => void }[];
		clear: (event: Event) => void;
	};

	type Props = {
		questionTypes: SelectOptionType<string>[];
		years: SelectOptionType<number>[];
		examCategories: SelectOptionType<string>[];
		examinerOrganizations: SelectOptionType<string>[];
		contractingOrganizations: SelectOptionType<string>[];
		areas: SelectOptionType<string>[];
		selectedQuestionTypes?: string[];
		selectedYears?: number[];
		selectedExamCategories?: string[];
		selectedExaminerOrganizations?: string[];
		selectedContractingOrganizations?: string[];
		selectedAreas?: string[];
		moreFiltersOpen?: boolean;
		onSearch: () => void;
	};

	let {
		questionTypes,
		years,
		examCategories,
		examinerOrganizations,
		contractingOrganizations,
		areas,
		selectedQuestionTypes = $bindable<string[]>([]),
		selectedYears = $bindable<number[]>([]),
		selectedExamCategories = $bindable<string[]>([]),
		selectedExaminerOrganizations = $bindable<string[]>([]),
		selectedContractingOrganizations = $bindable<string[]>([]),
		selectedAreas = $bindable<string[]>([]),
		moreFiltersOpen = $bindable(false),
		onSearch
	}: Props = $props();

	const moreFiltersPanelId = $props.id();

	let advancedFilterCount = $derived(
		selectedQuestionTypes.length +
			selectedExaminerOrganizations.length +
			selectedContractingOrganizations.length +
			selectedAreas.length
	);

	function toggleMoreFilters() {
		moreFiltersOpen = !moreFiltersOpen;
	}

	function getSelectedFilter<T>(
		label: string,
		selectedValues: T[],
		options: SelectOptionType<T>[],
		setSelectedValues: (values: T[]) => void
	): SelectedFilter | undefined {
		const items = selectedValues.flatMap((value) => {
			const option = options.find((item) => item.value === value);
			return option
				? [
						{
							key: `${label}-${String(value)}`,
							name: String(option.name),
							remove: (event: MouseEvent) => {
								event.preventDefault();
								setSelectedValues(
									selectedValues.filter((selectedValue) => selectedValue !== value)
								);
							}
						}
					]
				: [];
		});

		if (!items.length) return undefined;

		return {
			label,
			items,
			clear: (event: Event) => {
				event.preventDefault();
				setSelectedValues([]);
			}
		};
	}

	let selectedFilters = $derived(
		[
			getSelectedFilter(
				'Tipo',
				selectedQuestionTypes,
				questionTypes,
				(values) => (selectedQuestionTypes = values)
			),
			getSelectedFilter('Ano', selectedYears, years, (values) => (selectedYears = values)),
			getSelectedFilter(
				'Categoria',
				selectedExamCategories,
				examCategories,
				(values) => (selectedExamCategories = values)
			),
			getSelectedFilter(
				'Banca',
				selectedExaminerOrganizations,
				examinerOrganizations,
				(values) => (selectedExaminerOrganizations = values)
			),
			getSelectedFilter(
				'Instituição',
				selectedContractingOrganizations,
				contractingOrganizations,
				(values) => (selectedContractingOrganizations = values)
			),
			getSelectedFilter('Área', selectedAreas, areas, (values) => (selectedAreas = values))
		].filter((filter): filter is SelectedFilter => filter !== undefined)
	);
</script>

<section id="questions-filter-section" tabindex="-1" class="filter-container" aria-label="Filtros de questões">
	<div class="search-row">
		<div class="primary-filters">
			<CustomSelect
				items={years}
				bind:value={selectedYears}
				label="Ano"
				placeholder="Selecione um ano ..."
			/>

			<CustomSelect
				items={examCategories}
				bind:value={selectedExamCategories}
				label="Categoria do exame"
				placeholder="Selecione uma categoria de exame ..."
			/>
		</div>

		<button type="button" class="search-button" onclick={onSearch}>Pesquisar</button>
	</div>

	<div class="advanced-filters">
		<button
			type="button"
			class="advanced-filters-toggle"
			aria-expanded={moreFiltersOpen}
			aria-controls={moreFiltersPanelId}
			onclick={toggleMoreFilters}
		>
			<ChevronDownOutline class="advanced-filters-icon" />
			<span>Mais filtros</span>
			{#if advancedFilterCount > 0}
				<span class="advanced-filters-count" aria-label={`${advancedFilterCount} filtros avançados selecionados`}>
					{advancedFilterCount}
				</span>
			{/if}
		</button>

		{#if moreFiltersOpen}
			<div id={moreFiltersPanelId} class="remaining-filters">
				<CustomSelect
					items={questionTypes}
					bind:value={selectedQuestionTypes}
					label="Tipo de questão"
					placeholder="Selecione um tipo de questão ..."
				/>

				<SearchableSelect
					items={examinerOrganizations}
					bind:value={selectedExaminerOrganizations}
					label="Banca examinadora"
					placeholder="Selecione uma banca ..."
				/>

				<SearchableSelect
					items={contractingOrganizations}
					bind:value={selectedContractingOrganizations}
					label="Instituição"
					placeholder="Selecione uma instituição ..."
				/>

				<SearchableSelect
					items={areas}
					bind:value={selectedAreas}
					label="Área"
					placeholder="Selecione uma área ..."
				/>
			</div>
		{/if}
	</div>

	<div class="selected-filters" aria-live="polite">
		{#if selectedFilters.length}
			<ul aria-label="Filtros selecionados">
				{#each selectedFilters as filter (filter)}
					<li>
						<Badge color="secondary" class="selected-filter-badge">
							<span class="filter-label">
								{filter.label}
								<CloseButton
									size="xs"
									color="none"
									name={`Remover todos os filtros de ${filter.label}`}
									onclick={filter.clear}
								/>
							</span>
							{#each filter.items as item (item.key)}
								<Badge color="primary" class="filter-value tertiary-filter-value">
									{item.name}
									<CloseButton
										size="xs"
										color="none"
										name={`Remover ${item.name}`}
										onclick={item.remove}
									/>
								</Badge>
							{/each}
						</Badge>
					</li>
				{/each}
			</ul>
		{/if}
	</div>
</section>

<style>
	.filter-container {
		display: grid;
		gap: 1.125rem;
		margin: 1rem 20px;
		border: 1px solid var(--border);
		border-radius: 0.875rem;
		background-color: var(--surface-elevated);
		box-shadow: 0 1px 3px color-mix(in srgb, var(--text) 6%, transparent);
		padding: 1.25rem 1.5rem;
	}

	.search-row {
		display: flex;
		flex-wrap: wrap;
		align-items: flex-end;
		gap: 1rem;
	}

	.primary-filters {
		display: flex;
		flex: 1 1 auto;
		flex-wrap: wrap;
		gap: 1rem;
	}

	.primary-filters > :global(*) {
		flex: 1 1 14rem;
		min-width: 12rem;
		max-width: 20rem;
	}

	.advanced-filters {
		display: grid;
		gap: 0.875rem;
		padding-top: 0.25rem;
		border-top: 1px solid var(--border);
	}

	.advanced-filters-toggle {
		display: inline-flex;
		width: fit-content;
		align-items: center;
		gap: 0.5rem;
		border: 1px solid transparent;
		border-radius: 0.5rem;
		background: transparent;
		color: var(--tertiary);
		cursor: pointer;
		font-size: 0.875rem;
		font-weight: 700;
		padding: 0.4rem 0.6rem;
		transition:
			background-color 0.18s ease,
			border-color 0.18s ease,
			color 0.18s ease;
	}

	.advanced-filters-toggle:hover {
		border-color: var(--border);
		background-color: var(--surface-hover, rgba(0, 0, 0, 0.04));
		color: var(--tertiary-hover);
	}

	.advanced-filters-toggle:focus-visible {
		outline: 2px solid var(--tertiary);
		outline-offset: 2px;
	}

	:global(.advanced-filters-icon) {
		width: 1rem;
		height: 1rem;
		transition: transform 0.18s ease;
	}

	.advanced-filters-toggle[aria-expanded='true'] :global(.advanced-filters-icon) {
		transform: rotate(180deg);
	}

	.advanced-filters-count {
		display: inline-flex;
		align-items: center;
		justify-content: center;
		min-width: 1.25rem;
		height: 1.25rem;
		border-radius: 999px;
		background-color: var(--secondary);
		color: var(--text);
		font-size: 0.6875rem;
		font-weight: 700;
		line-height: 1;
		padding: 0 0.3rem;
	}

	.remaining-filters {
		display: grid;
		grid-template-columns: repeat(3, minmax(0, 1fr));
		gap: 1rem;
	}

	.selected-filters {
		min-width: 0;
		color: var(--text-muted);
		font-size: 0.8125rem;
	}

	.selected-filters ul {
		display: flex;
		flex-wrap: wrap;
		gap: 0.5rem;
		margin: 0;
		padding: 0;
		list-style: none;
	}

	.selected-filters li {
		display: flex;
	}

	:global(.selected-filter-badge) {
		display: inline-flex;
		align-items: center;
		gap: 0.35rem;
		background-color: var(--primary) !important;
		opacity: 0.85;
		color: var(--text) !important;
	}

	:global(.selected-filter-badge:hover) {
		background-color: var(--primary-hover) !important;
	}

	.filter-label {
		font-weight: 700;
		font-size: 0.875rem;
		margin: 0.35rem;
	}

	:global(.filter-value) {
		display: inline-flex;
		align-items: center;
		gap: 0.1rem;
		font-weight: 500;
	}

	:global(.tertiary-filter-value) {
		background-color: var(--secondary) !important;
		color: var(--text) !important;
	}

	:global(.tertiary-filter-value:hover) {
		background-color: var(--secondary-hover) !important;
	}

	.search-button {
		flex: 0 0 auto;
		min-height: 2.75rem;
		border: 1px solid var(--secondary);
		border-radius: 0.5rem;
		background-color: var(--secondary);
		color: var(--text);
		cursor: pointer;
		font-size: 0.875rem;
		font-weight: 700;
		padding: 0.65rem 1.5rem;
		transition:
			background-color 0.2s ease,
			box-shadow 0.2s ease,
			transform 0.2s ease;
	}

	.search-button:hover {
		background-color: var(--secondary-hover);
	}

	.search-button:focus-visible {
		outline: none;
		box-shadow: 0 0 0 3px color-mix(in srgb, var(--secondary) 24%, transparent);
	}

	.search-button:active {
		background-color: var(--secondary-active);
		transform: translateY(1px);
	}

	@media (max-width: 900px) {
		.remaining-filters {
			grid-template-columns: repeat(2, minmax(0, 1fr));
		}
	}

	@media (max-width: 560px) {
		.filter-container {
			margin: 1rem 12px;
			padding: 1rem;
		}

		.search-row {
			flex-direction: column;
			align-items: stretch;
		}

		.primary-filters > :global(*) {
			max-width: none;
		}

		.remaining-filters {
			grid-template-columns: minmax(0, 1fr);
		}

		.search-button {
			width: 100%;
			color: var(--text);
		}
	}
</style>
