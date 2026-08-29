<script>
	import Hero from "$lib/components/home/Hero.svelte";
  import Categories from "$lib/components/home/Categories.svelte";
  import How from "$lib/components/home/How.svelte";
  import Features from "$lib/components/home/Features.svelte";
</script>
<svelte:head>
  <title>Estudaki</title>
  <meta
    name="description"
    content="Pratique questões de vestibulares, concursos públicos e OAB gratuitamente com o EstudaKi."
  />
</svelte:head>

<Hero />
<Categories />
<How />
<Features />
