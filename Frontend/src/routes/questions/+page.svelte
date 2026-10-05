<script lang="ts">
	import type { PageData } from './$types';
	import { goto } from '$app/navigation';
	import { resolve } from '$app/paths';
	import { page } from '$app/state';
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

		params.set('page', page.toString());

		goto(resolve(`/questions?${params.toString()}`));
	}

	function handlePageSizeChange(pageSize: number) {
		const params = new SvelteURLSearchParams(window.location.search);

		params.set('page', '1');
		params.set('pageSize', pageSize.toString());

		goto(resolve(`/questions?${params.toString()}`));
	}

	function handleSearch() {
		const params = new SvelteURLSearchParams();

		params.set('page', '1');
		params.set('pageSize', data.pageSize.toString());

		for (const value of selectedQuestionTypes) params.append('typeQuestions', value);
		for (const value of selectedYears) params.append('year', value.toString());
		for (const value of selectedExamCategories) params.append('examCategories', value);
		for (const value of selectedExaminerOrganizations) params.append('examinerOrganization', value);
		for (const value of selectedContractingOrganizations)
			params.append('contractingOrganization', value);
		for (const value of selectedAreas) params.append('mainAreas', value);
		for (const value of selectedSubAreas) params.append('subAreas', value);

		goto(resolve(`/questions?${params.toString()}`));
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

	let selectedQuestionTypes = $state<string[]>(page.url.searchParams.getAll('typeQuestions'));
	let selectedYears = $state<number[]>(
		page.url.searchParams.getAll('year').map(Number).filter(Number.isInteger)
	);
	let selectedExamCategories = $state<string[]>(page.url.searchParams.getAll('examCategories'));
	let selectedExaminerOrganizations = $state<string[]>(
		page.url.searchParams.getAll('examinerOrganization')
	);
	let selectedContractingOrganizations = $state<string[]>(
		page.url.searchParams.getAll('contractingOrganization')
	);
	let selectedAreas = $state<string[]>(page.url.searchParams.getAll('mainAreas'));
	let selectedSubAreas = $state<string[]>(page.url.searchParams.getAll('subAreas'));
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
