<script lang="ts">
	import type { PageData } from './$types';
	import { goto } from '$app/navigation';
	import { resolve } from '$app/paths';
	import { SvelteURLSearchParams } from 'svelte/reactivity';
	import { QuestionType } from '$lib/questions/types/question-types';
	import { ExamCategory } from '$lib/questions/types/exam-category';
	import { getExamCategoryLabel, getQuestionTypeLabel } from '$lib/questions/types/labels';
	import FilterSection from '$lib/questions/components/FilterSection.svelte';
	import QuestionRender from '$lib/questions/components/QuestionRender.svelte';
	import Pagination from '$lib/questions/components/Pagination.svelte';

	let { data }: { data: PageData } = $props();

	function handlePageChange(page: number) {
		const params = new SvelteURLSearchParams(window.location.search);

		params.set('pageIndex', page.toString());

		goto(resolve(`/questions?${params.toString()}`));
	}

	function handlePageSizeChange(pageSize: number) {
		const params = new SvelteURLSearchParams(window.location.search);

		params.set('pageIndex', '1');
		params.set('pageSize', pageSize.toString());

		goto(resolve(`/questions?${params.toString()}`));
	}

	function handleSearch() {
		handlePageChange(1);
	}

	const questionTypes = Object.values(QuestionType).map((type) => ({
		value: type,
		name: getQuestionTypeLabel(type)
	}));
	const years = $derived(
		data.filterParameters.year!.map((year) => ({
			value: year,
			name: year.toString()
		}))
	);

	const examCategories = $derived(
		Object.values(ExamCategory).map((category) => ({
			value: category,
			name: getExamCategoryLabel(category)
		}))
	);

	const examinerOrganizations = $derived(
		data.filterParameters.examinerOrganization!.map((name) => ({
			value: name,
			name
		}))
	);

	const contractingOrganizations = $derived(
		data.filterParameters.contractingOrganization!.map((name) => ({
			value: name,
			name
		}))
	);

	const areas = $derived(
		data.filterParameters.mainAreas!.map((name) => ({
			value: name,
			name
		}))
	);

	const subAreas = $derived(
		data.filterParameters.subAreas!.map((name) => ({
			value: name,
			name
		}))
	);

	let selectedQuestionTypes = $state<string[]>([]);
	let selectedYears = $state<number[]>([]);
	let selectedExamCategories = $state<string[]>([]);
	let selectedExaminerOrganizations = $state<string[]>([]);
	let selectedContractingOrganizations = $state<string[]>([]);
	let selectedAreas = $state<string[]>([]);
	let selectedSubAreas = $state<string[]>([]);
</script>

<svelte:head>
	<title>Estudaki - Questões</title>
</svelte:head>

<FilterSection
	{questionTypes}
	{years}
	{examCategories}
	{examinerOrganizations}
	{contractingOrganizations}
	{areas}
	{subAreas}
	bind:selectedQuestionTypes
	bind:selectedYears
	bind:selectedExamCategories
	bind:selectedExaminerOrganizations
	bind:selectedContractingOrganizations
	bind:selectedAreas
	bind:selectedSubAreas
	onSearch={handleSearch}
/>

<box gap={4}>
	<Pagination
		pageNumber={data.pageNumber}
		pageSize={data.pageSize}
		totalPages={data.totalPages}
		{handlePageChange}
		{handlePageSizeChange}
	/>

	{#each data.items as question (question.questionId)}
		<QuestionRender class="question" {question} />
	{/each}

	<Pagination
		pageNumber={data.pageNumber}
		pageSize={data.pageSize}
		totalPages={data.totalPages}
		{handlePageChange}
		{handlePageSizeChange}
	/>
</box>

<style>
	:global(.question) {
		padding: 1rem;
		margin: 1rem 20px;
		margin-bottom: 1rem;
	}
</style>
