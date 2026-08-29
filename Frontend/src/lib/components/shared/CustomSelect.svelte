<script lang="ts" generics="T">
    import { Label, MultiSelect, type SelectOptionType } from 'flowbite-svelte';

    type Props = {
        items: SelectOptionType<T>[];
        value?: T[];
        label?: string;
        placeholder?: string;
    };

    let {
        items,
        value = $bindable<T[]>([]),
        label,
        placeholder = 'Selecione...',
    }: Props = $props();
</script>

<div class="select-container">    
    <Label
        for="select"
        class="select-label"
    >
        {#if label}
            {label}
        {/if}
        <MultiSelect
            id="select"
            {items}
            bind:value
            {placeholder}
            class="select mt-1"
            dropdownClass="select-dropdown"
        >
            {#snippet children({ item })}
                {#if value.length > 0 && item.value === value[0]}
                    <div class="flex items-center gap-1">
                        <span class="selected-count">
                            {value.length}
                            {value.length === 1
                                ? ' selecionado'
                                : ' selecionados'}
                        </span>                    
                    </div>
                {/if}
            {/snippet}            
        </MultiSelect>
    </Label>    
</div>


<style>
    :global(.select){
        border-color: var(--secondary);        
        border-width: 2px;
        font-size: 0.875rem;
    }    

    :global(.select:focus) {
        border-color: none;
        border-width: 2px;
    }

    :global(.select:hover) {
        border-color: var(--secondary-hover);        
        border-width: 2px;
    }   

    :global(.select span){        
        color: var(--font-color);
        font-size: 0.9rem;
    }
  
    :global(.select-label){
        font-size: 0.875rem;     
    }

    /* :global(.select-dropdown) {
        background-color: #e16c6c;
        border: 1px solid #7DA184;
        border-radius: 0.5rem;
        box-shadow: 0 10px 20px rgba(0, 0, 0, 0.08);
    }

    :global(.select-dropdown li) {
        padding: 0.5rem 0.75rem;
        color: #374151;
        cursor: pointer;
    }

    :global(.select-dropdown li:hover) {
        background-color: #E3E2DA;
        color: #5F8066;
    } */
</style>