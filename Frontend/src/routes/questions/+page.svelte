<script lang="ts">
	import type { PageData } from './$types';
	import { goto } from '$app/navigation';
    import { QuestionType } from '$lib/questions/types/question-types';
    import { getExamCategoryLabel, getQuestionTypeLabel } from '$lib/questions/types/labels';
    import { resolve } from '$app/paths';
	import { PaginationNav, Button } from 'flowbite-svelte';
    import { SvelteURLSearchParams } from 'svelte/reactivity';
	import {
		ArrowLeftOutline,
		ArrowRightOutline,
        ClipboardCheckOutline,
        FlagOutline
	} from 'flowbite-svelte-icons';
	import { ExamCategory } from '$lib/questions/types/exam-category';
	import CustomSelect from '$lib/components/shared/CustomSelect.svelte';

	let { data }: { data: PageData } = $props();

    function handlePageChange(page: number) {
        const params = new SvelteURLSearchParams(window.location.search);

        params.set('pageIndex', page.toString());

        goto(resolve(`/questions?${params.toString()}`));
    }    

    // Filter Section
    const questionTypes = Object.values(QuestionType).map((type) => ({
        value: type,
        name: getQuestionTypeLabel(type)
    }));
    const examCategories = Object.values(ExamCategory).map((type) => ({
        value: type,
        name: getExamCategoryLabel(type)
    }));
    let yearsAvailable: number[] = [2022, 2023, 2024];
    let examinerOrganizationList: string[] = ["Teste1", "Teste2", "Teste3"];
    let contractingOrganizationList: string[] = ["Teste1", "Teste2", "Teste3"];
    let areaList: string[] = ["Teste1", "Teste2", "Teste3"];
    let subAreaList: string[] = ["Teste1", "Teste2", "Teste3"];



    let selectedQuestionTypes = $state<string[]>([]);
    let selectedExamCategories = $state<string[]>([]);
    let selectedExaminerOrganizations = $state<string[]>([]);
    let selectedContractingOrganizations = $state<string[]>([]);
    let selectedAreas = $state<string[]>([]);
    let selectedSubAreas = $state<string[]>([]);
    let selectedYears = $state<number[]>([]);   
</script>

<CustomSelect
    items={yearsAvailable.map((year) => ({ value: year, name: year.toString() }))}
    bind:value={selectedYears}
    label="Ano"
    placeholder="Selecione um ano ..."    
/>  

<CustomSelect
    items={questionTypes}
    bind:value={selectedQuestionTypes}
    label="Tipo de questão"
    placeholder="Selecione um tipo de questão ..."
    />

<CustomSelect
    items={examCategories}
    bind:value={selectedExamCategories}
    label="Categoria do exame"
    placeholder="Selecione uma categoria de exame ..."
/>

<CustomSelect
    items={examinerOrganizationList.map((org) => ({ value: org, name: org }))}
    bind:value={selectedExaminerOrganizations}
    label="Banca examinadora"
    placeholder="Selecione uma banca ..."
/>

<CustomSelect
    items={contractingOrganizationList.map((org) => ({ value: org, name: org }))}
    bind:value={selectedContractingOrganizations}
    label="Instituição"
    placeholder="Selecione uma instituição ..."
/>

<CustomSelect
    items={areaList.map((org) => ({ value: org, name: org }))}
    bind:value={selectedAreas}
    label="Área"
    placeholder="Selecione uma área ..."
/>

<CustomSelect
    items={subAreaList.map((org) => ({ value: org, name: org }))}
    bind:value={selectedSubAreas}
    label="Subárea"
    placeholder="Selecione uma subárea ..."
/>

<box gap={4}>

	<text>{data.totalItems} questões encontradas</text>

	<PaginationNav
		visiblePages={7}
		currentPage={data.pageNumber}
		totalPages={data.totalPages}
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

	{#each data.items as question (question.questionId)}
		<div class="question-card">
            <div class="question-card-head">
                <div class="public-notice-info">
                    <h3 class="year">Ano: <span>{question.year}</span></h3>
                    <h3 class="exam-category">Categoria: <span>{question.examCategory}</span></h3>
                    <h3 class="examiner-organization">Banca: <span>{question.examinerOrganization}</span></h3>
                    <h3 class="examiner-organization">Organizadora: <span>{question.contractingOrganization}</span></h3>
                    <div class="header-actions">
                        <Button class="copy-question">
                            <ClipboardCheckOutline class="shrink-0 h-6 w-6" />
                        </Button>                                            
                        <Button class="signalize-question">
                            <FlagOutline class="shrink-0 h-6 w-6" />
                        </Button>                        
                    </div>             
                </div>
                <div class="exam-info">
                    <h3 class="public-notice-number">Edital: <span>   {question.publicNoticeNumber}</span></h3>
                    <h3 class="phase">Fase: <span>{question.phase}</span></h3>
                </div>
                <div>                
                    <h3 class="positions">{question.positions.toString()}</h3>
                </div>    
            </div>

            <div class="content">
                <div class="question-support">
                    {#each question.questionSupports as support (support.id)}
                        {#each support.contents as content (content.order)}                    
                            {#if (content.type === 'image' && content.key)}
                                <div class="question-support-content">
                                    <img src={content.key} alt="" />
                                </div>
                            {:else if (content.type === 'paragraph' && content.text)}
                                <div class="question-support-title">
                                    <h3>{content.title}</h3>
                                </div>                            
                                <div class="question-support-content">
                                    <h3>{content.text}</h3>
                                </div>

                                <div class="question-support-source">
                                    <h3>{content.source}</h3>
                                </div>
                            {/if}                    
                        {/each}        
                    {/each}
                </div> 

                <div class="question-content">
                    {#each question.questionContents as content (content.order)}
                        {#if (content.type === 'image' && content.key)}
                            <div class="question-content-item">
                                <img src={content.key} alt="{content.description}" />
                            </div>
                        {:else if (content.type === 'paragraph' && content.text)}
                            <div class="question-content-item">
                                <h3>{content.title}</h3>
                            </div>                            
                            <div class="question-content-item">
                                <h3>{content.text}</h3>
                            </div>

                            <div class="question-content-item">
                                <h3>{content.source}</h3>
                            </div>
                        {/if}
                    {/each}
                </div>
            </div>			
		</div>        
	{/each}

	<PaginationNav
		visiblePages={7}
		currentPage={data.pageNumber}
		totalPages={data.totalPages}
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
</box>

<style>
    .question-card {
        padding: 1rem;
        margin: 1rem 20px;
        margin-bottom: 1rem;
    }    

    .positions {
        grid-area: positions;
    }

    .public-notice-info, .exam-info {
        display: flex;        
        font-size: 0.875rem;
        align-items: center;
        margin-bottom: 0.5rem;
    }

    .public-notice-info h3, .exam-info h3 {
        color: var(--font-color);
        font-weight: 500;
    }

    .public-notice-info span, .exam-info span {
        color: var(--secondary);      
        size: 0.875rem;  
        font-weight: 400;
    }

    .header-actions {
        display: flex;
        size: 0.875rem;
    }

    .question-support {
        margin-bottom: 1rem;
    }

    .question-support-content, 
    .question-content-item {
        margin-bottom: 0.5rem;
        text-align: justify;
    }

    :global(.signalize-question, .copy-question) {
        background-color: transparent;
        color: var(--tertiary);
        cursor: pointer;        
        padding: 0px 10px;
        border: none;
        outline: none;
        box-shadow: none;

        transition: color 0.2s ease, transform 0.2s ease-in-out;        
    }
    :global(.signalize-question:active, .copy-question:active) {
        transform: scale(1.5);
    }

    :global(.signalize-question:hover, .copy-question:hover) {
        color: var(--tertiary-hover);
    }

    :global(.signalize-question:focus, .copy-question:focus) {
        outline: none;
        box-shadow: none;
        border: none;
    }

    :global(.signalize-question:focus-visible, .copy-question:focus-visible) {
        outline: none;
        box-shadow: none;
        border: none;
    }   

    span {
        font-weight: bold;
        font-size: 0.875rem;
        margin-right: 0.25rem;
    }

   
</style>

