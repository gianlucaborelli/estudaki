<script lang="ts">
	import { PaginationNav } from 'flowbite-svelte';
	import { ArrowLeftOutline, ArrowRightOutline } from 'flowbite-svelte-icons';

	type Props = {
		pageNumber: number;
		pageSize: number;
		totalPages: number;
		handlePageChange: (page: number) => void;
		handlePageSizeChange: (pageSize: number) => void;
	};

	let { pageNumber, pageSize, totalPages, handlePageChange, handlePageSizeChange }: Props =
		$props();

	function handlePageSizeSelect(event: Event) {
		handlePageSizeChange(Number((event.currentTarget as HTMLSelectElement).value));
	}
</script>

<nav class="question-pagination" aria-label="Paginação das questões">
	<div class="pagination-pages">
		<PaginationNav
			visiblePages={7}
			currentPage={pageNumber}
			{totalPages}
			onPageChange={handlePageChange}
		>
			{#snippet prevContent()}
				<span class="sr-only">Anterior</span>
				<ArrowLeftOutline class="h-5 w-5" />
			{/snippet}

			{#snippet nextContent()}
				<span class="sr-only">Próxima</span>
				<ArrowRightOutline class="h-5 w-5" />
			{/snippet}
		</PaginationNav>
	</div>

	<label class="page-size-control">
		<span class="sr-only">Questões por página</span>
		<select value={pageSize} onchange={handlePageSizeSelect} aria-label="Questões por página">
			<option value={5}>5</option>
			<option value={10}>10</option>
			<option value={15}>15</option>
			<option value={25}>25</option>
		</select>
	</label>
</nav>

<style>
	.question-pagination {
		padding: 0 1.5rem;
		display: grid;
		grid-template-columns: 1fr auto 1fr;
		align-items: center;
		width: 100%;
		margin: 1.5rem 0;
	}

	.pagination-pages {
		grid-column: 2;
	}

	.question-pagination :global(ul) {
		display: flex;
		flex-wrap: wrap;
		justify-content: center;
		gap: 0.375rem;
		margin: 0;
		padding: 0;
	}

	.question-pagination :global(a),
	.question-pagination :global(button) {
		display: inline-flex;
		align-items: center;
		justify-content: center;
		min-width: 2.5rem;
		height: 2.5rem;
		padding: 0 0.625rem;
		border: 1px solid var(--border);
		border-radius: 0.375rem;
		background-color: var(--surface);
		color: var(--text-muted);
		font-size: 0.875rem;
		font-weight: 600;
		line-height: 1;
		text-decoration: none;
		transition:
			background-color 0.18s ease,
			border-color 0.18s ease,
			color 0.18s ease;
	}

	.question-pagination :global(a:hover),
	.question-pagination :global(button:hover:not(:disabled)) {
		border-color: var(--primary);
		background-color: color-mix(in srgb, var(--primary) 12%, var(--surface));
		color: var(--text);
	}

	.question-pagination :global([aria-current='page']) {
		border-color: var(--secondary);
		border-radius: 0.375rem;
		border-width: 2px;
	}

	.question-pagination :global(button:disabled),
	.question-pagination :global([aria-disabled='true']) {
		cursor: not-allowed;
		opacity: 0.45;
	}

	.page-size-control {
		display: inline-flex;
		grid-column: 3;
		justify-self: end;
		align-items: center;
	}

	.page-size-control select {
		min-width: 3.75rem;
		height: 2.5rem;
		padding: 0 1.75rem 0 0.625rem;
		border: 1px solid var(--border);
		border-radius: 0.375rem;
		background-color: var(--surface);
		color: var(--text);
		font: inherit;
		cursor: pointer;
	}

	.page-size-control select:hover {
		border-color: var(--primary);
	}

	@media (max-width: 640px) {
		.question-pagination {
			grid-template-columns: 1fr;
			gap: 1rem;
			margin: 1.25rem 0;
		}

		.pagination-pages,
		.page-size-control {
			grid-column: 1;
			justify-self: center;
		}

		.question-pagination :global(ul) {
			gap: 0.25rem;
		}

		.question-pagination :global(a),
		.question-pagination :global(button) {
			min-width: 2.25rem;
			height: 2.25rem;
			padding: 0 0.5rem;
		}
	}
</style>
