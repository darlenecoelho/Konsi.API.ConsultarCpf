<template>
  <div class="isolate bg-white px-6 py-24 sm:py-32 lg:px-8">
    <Header />

    <div class="mx-auto mt-8 max-w-xl sm:mt-10">
      <div class="grid grid-cols-1 gap-x-8 gap-y-6 sm:grid-cols-1">
        <div>
          <label for="first-name" class="block text-sm font-semibold leading-6 text-gray-900">Cadastro de Pessoas Física</label>
          <div class="mt-2.5">
            <input
                type="text"
                name="cpf"
                id="cpf"
                v-model="input.cpf"
                placeholder="000.000.000-00"
                class="block w-full rounded-md border-0 px-3.5 py-2 text-gray-900 shadow-sm ring-1 ring-inset ring-gray-300 placeholder:text-gray-400 focus:ring-2 focus:ring-inset focus:ring-indigo-600 sm:text-sm sm:leading-6">
          </div>
        </div>
      </div>
      <div class="mt-5">
        <button @click.prevent="getCpf" type="submit" :disabled="loading" class="block w-32 rounded-md bg-green-800 px-3.5 py-2.5 text-center text-sm font-semibold text-white shadow-sm hover:bg-green-900 focus-visible:outline focus-visible:outline-2 focus-visible:outline-offset-2 focus-visible:outline-indigo-600">
          {{ loading ? 'Consultando...' : 'Consultar' }}
        </button>
      </div>
    </div>

    <Modal v-if="modalState.modalDynamic" :cpf="listCpf.cpf" :benefit="listCpf.beneficios "/>

  </div>

</template>

<script setup>
import Header from "@/components/Header.vue";
import axios from "axios";
import {computed, reactive, ref} from "vue";
import {useToast} from 'vue-toast-notification';
import 'vue-toast-notification/dist/theme-sugar.css';
import Modal from "@/components/Modal.vue";

import {useModalCpf} from "@/store/ModalState.js";

const $toast = useToast();

const listCpf = ref([]);
const loading = ref(false);
const modalState = useModalCpf();


const input = reactive({
  cpf: "",
});

async function getCpf() {
  loading.value = true;
  try {
    const response = await axios.get(`elasticsearch/${input.cpf}`);
    listCpf.value = response.data.data;
    console.log(listCpf.value)
    modalState.modalOpen();
  } catch (error) {
    if (error.response && error.response.status === 404) {
      modalState.modalClose();
      $toast.error('CPF não encontrado', {
        position: 'top-right',
        duration: 5000,
      });
    }
  } finally {
    loading.value = false;
  }
}

</script>




