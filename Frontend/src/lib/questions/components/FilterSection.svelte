<script lang="ts">
	import CustomSelect from '$lib/components/shared/CustomSelect.svelte';
	import { Badge, CloseButton, type SelectOptionType } from 'flowbite-svelte';

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
		subAreas: SelectOptionType<string>[];
		selectedQuestionTypes?: string[];
		selectedYears?: number[];
		selectedExamCategories?: string[];
		selectedExaminerOrganizations?: string[];
		selectedContractingOrganizations?: string[];
		selectedAreas?: string[];
		selectedSubAreas?: string[];
		onSearch: () => void;
	};

	let {
		questionTypes,
		years,
		examCategories,
		examinerOrganizations,
		contractingOrganizations,
		areas,
		subAreas,
		selectedQuestionTypes = $bindable<string[]>([]),
		selectedYears = $bindable<number[]>([]),
		selectedExamCategories = $bindable<string[]>([]),
		selectedExaminerOrganizations = $bindable<string[]>([]),
		selectedContractingOrganizations = $bindable<string[]>([]),
		selectedAreas = $bindable<string[]>([]),
		selectedSubAreas = $bindable<string[]>([]),
		onSearch
	}: Props = $props();

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
			getSelectedFilter('Área', selectedAreas, areas, (values) => (selectedAreas = values)),
			getSelectedFilter(
				'Subárea',
				selectedSubAreas,
				subAreas,
				(values) => (selectedSubAreas = values)
			)
		].filter((filter): filter is SelectedFilter => filter !== undefined)
	);
</script>

<section class="filter-container" aria-label="Filtros de questões">
	<div class="question-type-filter">
		<CustomSelect
			items={questionTypes}
			bind:value={selectedQuestionTypes}
			label="Tipo de questão"
			placeholder="Selecione um tipo de questão ..."
		/>
	</div>

	<div class="remaining-filters">
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

		<CustomSelect
			items={examinerOrganizations}
			bind:value={selectedExaminerOrganizations}
			label="Banca examinadora"
			placeholder="Selecione uma banca ..."
		/>

		<CustomSelect
			items={contractingOrganizations}
			bind:value={selectedContractingOrganizations}
			label="Instituição"
			placeholder="Selecione uma instituição ..."
		/>

		<CustomSelect
			items={areas}
			bind:value={selectedAreas}
			label="Área"
			placeholder="Selecione uma área ..."
		/>

		<CustomSelect
			items={subAreas}
			bind:value={selectedSubAreas}
			label="Subárea"
			placeholder="Selecione uma subárea ..."
		/>
	</div>

	<div class="filter-actions">
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
			{:else}
				<span>Nenhum filtro selecionado</span>
			{/if}
		</div>

		<button type="button" class="search-button" onclick={onSearch}>Pesquisar</button>
	</div>
</section>

<style>
	.filter-container {
		display: grid;
		gap: 1rem;
		padding: 1rem 1.25rem;
	}

	.question-type-filter {
		width: fit-content;
		max-width: 100%;
	}

	.remaining-filters {
		display: grid;
		grid-template-columns: repeat(3, minmax(0, 1fr));
		gap: 1rem;
	}

	.filter-actions {
		display: flex;
		align-items: center;
		justify-content: space-between;
		gap: 1rem;
		padding-top: 1rem;
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
		color: var(--surface-elevated) !important;
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
		color: var(--surface-elevated) !important;
	}

	:global(.tertiary-filter-value:hover) {
		background-color: var(--secondary-hover) !important;
	}

	.search-button {
		flex: 0 0 auto;
		border: 1px solid var(--secondary);
		border-radius: 0.5rem;
		background-color: var(--secondary);
		color: var(--surface-elevated);
		cursor: pointer;
		font-size: 0.875rem;
		font-weight: 700;
		padding: 0.65rem 1.15rem;
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
			padding: 1rem;
		}

		.question-type-filter,
		.remaining-filters {
			width: 100%;
		}

		.remaining-filters {
			grid-template-columns: minmax(0, 1fr);
		}

		.filter-actions {
			align-items: stretch;
			flex-direction: column;
		}

		.search-button {
			width: 100%;
		}
	}
</style>
