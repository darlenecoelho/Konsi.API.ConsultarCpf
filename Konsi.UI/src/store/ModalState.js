import {defineStore} from 'pinia';

export const useModalCpf = defineStore('Modal', {

    state: () => ({
        showModal: false
    }),

    getters: {
        modalDynamic: (state) => state.showModal,
    },

    actions: {
        modalOpen() {
            this.showModal = true;
        },
        modalClose() {
            this.showModal = false;
        }
    }

})