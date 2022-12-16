<template>
  <TransitionRoot appear :show="props.open" as="template">
    <Dialog as="div" @close="closeModal(false)" class="relative z-10">
      <TransitionChild
        as="template"
        enter="duration-300 ease-out"
        enter-from="opacity-0"
        enter-to="opacity-100"
        leave="duration-200 ease-in"
        leave-from="opacity-100"
        leave-to="opacity-0"
      >
        <div class="fixed inset-0 bg-black bg-opacity-25" />
      </TransitionChild>

      <div class="fixed inset-0 overflow-y-auto">
        <div
          class="flex min-h-full items-center justify-center p-4 text-center"
        >
          <TransitionChild
            as="template"
            enter="duration-300 ease-out"
            enter-from="opacity-0 scale-95"
            enter-to="opacity-100 scale-100"
            leave="duration-200 ease-in"
            leave-from="opacity-100 scale-100"
            leave-to="opacity-0 scale-95"
          >
            <DialogPanel
              class="w-full max-w-md transform overflow-hidden rounded bg-white p-6 text-left align-middle shadow-xl transition-all"
            >
              <DialogTitle
                as="h3"
                class="text-lg font-medium leading-6 text-gray-900"
              >
                Delete credential {{ id }}
              </DialogTitle>
              <DialogDescription>
                This will deactivate the account on Kerbero.
              </DialogDescription>
              <div class="mt-2">
                <p class="text-sm text-gray-500">
                  Are you sure to delete the credential? The smart-lock and the
                  keys associated will be unusable.
                </p>
              </div>

              <div class="flex justify-between flex-row-reverse mt-4 mx-4">
                <button
                  type="button"
                  class="inline-flex justify-center rounded border border-transparent bg-purple-100 px-4 py-2 text-sm font-medium text-purple-800 hover:bg-purple-200 focus:outline-none focus-visible:ring-2 focus-visible:ring-purple-500 focus-visible:ring-offset-2"
                  @click="closeModal(true)"
                >
                  Delete
                </button>
                <button
                  type="button"
                  class="inline-flex justify-center rounded border border-slate-400 px-4 py-2 text-sm font-medium text-slate-800 hover:bg-slate-200 focus:outline-none focus-visible:ring-2 focus-visible:ring-slate-500"
                  @click="closeModal(false)"
                >
                  Cancel
                </button>
              </div>
            </DialogPanel>
          </TransitionChild>
        </div>
      </div>
    </Dialog>
  </TransitionRoot>
</template>

<script setup lang="ts">
import {
  TransitionRoot,
  TransitionChild,
  Dialog,
  DialogPanel,
  DialogTitle,
  DialogDescription,
} from "@headlessui/vue";

const props = defineProps<{
  open: boolean;
  id: number;
}>();

const emit = defineEmits<{
  (e: "onClose", isConfirmed: boolean, id: number): void;
}>();

function closeModal(confirmation: boolean) {
  emit("onClose", confirmation, props.id);
}
</script>
