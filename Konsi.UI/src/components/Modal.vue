<template>
  <div class="relative z-10" aria-labelledby="modal-title" role="dialog" aria-modal="true">
    <div class="fixed inset-0 bg-gray-500 bg-opacity-75 transition-opacity"></div>
    <div class="fixed inset-0 z-10 w-screen overflow-y-auto">
      <div class="flex min-h-full items-end justify-center p-4 text-center sm:items-center sm:p-0">
        <div class="relative transform overflow-hidden rounded-lg bg-white text-left shadow-xl transition-all sm:my-8 sm:w-full sm:max-w-lg">
          <div class="bg-white px-4 pb-4 pt-5 sm:p-6 sm:pb-4">
            <div class="sm:flex sm:items-start">
              <div class="mt-3 text-center sm:ml-4 sm:mt-0 sm:text-left">
                <h3 class="text-xl leading-6 text-gray-600 mb-4" id="modal-title">Informações gerais</h3>
                <div class="mt-2">
                  <div>
                    <h1 class="text-sm mb-2"><span class="font-semibold">Número do CPF: </span>{{ props.cpf }}</h1>
                    <ul>
                      <span class="text-sm font-semibold">{{ props.benefit.length > 1 ? 'Números de benefícios e codigos tipo beneficios:' : 'Número de benefício e codigo tipo beneficio'  }}</span>
                      <li v-for="benefitItem in props.benefit" :key="benefitItem.numero_beneficio">
                       <p>{{ benefitItem.numero_beneficio }} - {{ benefitItem.codigo_tipo_beneficio }}</p>
                      </li>
                    </ul>
                  </div>
                </div>
              </div>
            </div>
          </div>
          <div class="bg-gray-50 px-4 py-3 sm:flex sm:flex-row-reverse sm:px-6">
            <button @click="modal.modalClose()" type="button" class="mt-3 inline-flex w-full justify-center rounded-md bg-white px-3 py-2 text-sm font-semibold text-gray-900 shadow-sm ring-1 ring-inset ring-gray-300 hover:bg-gray-50 sm:mt-0 sm:w-auto">Fechar</button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>

import { useModalCpf } from "@/store/ModalState.js";

const modal = useModalCpf();

const props = defineProps({
  cpf: String,
  benefit: Array,
});

</script>