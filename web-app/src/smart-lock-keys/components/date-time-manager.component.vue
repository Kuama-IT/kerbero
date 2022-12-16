<template>
  <div class="flex justify-center items-center">
    <Popover class="w-full">
      <PopoverButton class="w-full">
        <DateTimeDisplay
          class="grow"
          :start-date="startDate"
          :end-date="endDate"
        ></DateTimeDisplay>
      </PopoverButton>
      <PopoverPanel class="absolute">
        <DateTimePicker
          :start-date="startDate"
          :end-date="endDate"
          @date-selected="manageDate"
          @date-canceled="updateMode = false"
        ></DateTimePicker>
      </PopoverPanel>
    </Popover>
  </div>
</template>

<script setup lang="ts">
import DateTimeDisplay from "@/smart-lock-keys/components/date-time-display.component.vue";
import DateTimePicker from "@/smart-lock-keys/components/date-time-picker.component.vue";
import { ref } from "vue";
import { Popover, PopoverButton, PopoverPanel } from "@headlessui/vue";

const props = defineProps<{
  startDate: Date;
  endDate: Date;
}>();

const emit = defineEmits<{
  (e: "datesUpdated", date: { from: Date; to: Date }): void;
  (e: "updating"): void;
}>();

const startDate = ref(props.startDate);
const endDate = ref(props.endDate);

const updateMode = ref(false);

const manageDate = (dates: { from: Date; to: Date }) => {
  startDate.value = dates.from;
  endDate.value = dates.to;
  updateMode.value = false;
  emit("datesUpdated", dates);
};
</script>
