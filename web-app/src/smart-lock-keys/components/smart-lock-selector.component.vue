<template>
  <div class="relative w-72">
    <Combobox :model-value="selected" @update:model-value="onChange">
      <div class="relative mt-1">
        <div
          class="relative w-full cursor-default overflow-hidden rounded bg-white text-left shadow-md focus:outline-none focus-visible:ring-2 focus-visible:ring-white focus-visible:ring-opacity-75 focus-visible:ring-offset-2 focus-visible:ring-offset-teal-300 sm:text-sm"
        >
          <ComboboxInput
            class="w-full border-none py-2 pl-3 pr-10 text-sm leading-5 text-gray-900 focus:ring-0"
            :display-value="(smartLock) => smartLock.name"
          />
          <ComboboxButton
            class="absolute inset-y-0 right-0 flex items-center pr-2"
          >
            <ChevronUpDownIcon
              class="h-5 w-5 text-gray-400"
              aria-hidden="true"
            />
          </ComboboxButton>
        </div>
        <TransitionRoot
          leave="transition ease-in duration-100"
          leave-from="opacity-100"
          leave-to="opacity-0"
          @after-leave="query = ''"
        >
          <ComboboxOptions
            class="absolute mt-1 max-h-60 w-full overflow-auto rounded bg-white py-1 text-base shadow ring-1 ring-black ring-opacity-5 focus:outline-none sm:text-sm"
          >
            <div
              v-if="filteredSmartLocks.length === 0 && query !== ''"
              class="relative cursor-default select-none py-2 px-4 text-gray-700"
            >
              Nothing found.
            </div>

            <ComboboxOption
              v-for="smartLock in filteredSmartLocks"
              as="template"
              :key="smartLock.id"
              :value="smartLock"
              v-slot="{ selected: itemSelected, active }"
            >
              <li
                class="relative cursor-default select-none py-2 pl-10 pr-4"
                :class="{
                  'bg-purple-500 text-white': active,
                  'text-gray-900': !active,
                }"
              >
                <span
                  class="block truncate"
                  :class="{
                    'font-medium': itemSelected,
                    'font-normal': !itemSelected,
                  }"
                >
                  {{ smartLock.name }}
                </span>
                <span
                  v-if="itemSelected"
                  class="absolute inset-y-0 left-0 flex items-center pl-3"
                  :class="{ 'text-white': active, 'text-purple-600': !active }"
                >
                  <CheckIcon class="h-5 w-5" aria-hidden="true" />
                </span>
              </li>
            </ComboboxOption>
          </ComboboxOptions>
        </TransitionRoot>
      </div>
    </Combobox>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, toRef } from "vue";
import {
  Combobox,
  ComboboxInput,
  ComboboxButton,
  ComboboxOptions,
  ComboboxOption,
  TransitionRoot,
} from "@headlessui/vue";
import { CheckIcon, ChevronUpDownIcon } from "@heroicons/vue/24/solid";
import {
  SmartLockListResponseDto,
  SmartLockResponseDto,
} from "@/smart-locks/api/smart-lock.schemas";

const props = defineProps<{
  selection: SmartLockResponseDto;
  items: SmartLockListResponseDto;
}>();

const emit = defineEmits<{
  (e: "update:selection", selection: SmartLockResponseDto): void;
}>();

let selected = toRef(props, "selection");

const smartLocks = ref(props.items.smartLocks);
let query = ref("");

let filteredSmartLocks = computed(() => {
  return query.value === ""
    ? smartLocks.value
    : smartLocks.value.filter((smartLock) =>
        smartLock.name?.toLowerCase().includes(query.value.toLowerCase())
      );
});

const onChange = (queried: any) => {
  emit("update:selection", queried);
};
</script>
