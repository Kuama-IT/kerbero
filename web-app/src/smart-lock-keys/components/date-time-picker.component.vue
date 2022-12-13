<template>
  <Datepicker
    v-model="dates"
    @internal-model-change="handleDate"
    ref="dp"
    inline
    range
    :partial-range="false"
    :enable-time-picker="false"
  >
    <template #action-row>
      <div class="action-row flex flex-row-reverse text-sm">
        <button
          class="basis-1/2 py-2 ml-1 mr-2 my-4 rounded bg-purple-600 text-white disabled:bg-purple-300"
          @click="selectDate"
          :disabled="settable"
        >
          Set
        </button>
        <button
          class="basis-1/2 py-2 ml-2 mr-1 my-4 rounded text-gray-500 outline outline-1 outline-[#ddd]"
          @click="closeMenu"
        >
          Cancel
        </button>
      </div>
    </template>
  </Datepicker>
</template>

<script setup lang="ts">
import Datepicker from "@vuepic/vue-datepicker";
import { onMounted, ref } from "vue";

const emit = defineEmits<{
  (e: "date-selected", dates: { from: Date; to: Date }): void;
  (e: "date-canceled"): void;
}>();

const props = defineProps<{
  startDate: Date;
  endDate: Date;
}>();

const dates = ref();
const dp = ref();

const settable = ref(true);

onMounted(() => {
  dates.value = [props.startDate, props.endDate];
});

const handleDate = (handleDates: any) => {
  if (handleDates != undefined && handleDates.length > 1) {
    settable.value = false;
  }
};

const selectDate = () => {
  dp.value.selectDate();
  emit("date-selected", { from: dates.value[0], to: dates.value[1] });
};

const closeMenu = () => {
  dp.value.closeMenu();
  emit("date-canceled");
};
</script>

<style lang="scss">
$dp__font_family: "Allerta", monospace;
$dp__font_size: 0.75rem;
$dp__cell_padding: 15px;
$dp__cell_size: 10px !default;
$dp__month_year_row_height: 50px;
$dp__border_radius: 15px !default;
$dp__month_year_row_button_size: 25px;
$dp__cell_border_radius: 10px;
$dp__menu_min_width: 275px;
$dp__row_margin: 10px 0 !default;

@import "@vuepic/vue-datepicker/src/VueDatePicker/style/main.scss";

.dp__theme_light {
  --dp-hover-color: #fff;
  --dp-hover-color-range: rgb(216 180 254);
  --dp-hover-text-color: #212121;
  --dp-hover-icon-color: #8f8f8f;
  --dp-primary-color: rgb(147 51 234);
  --dp-primary-disabled-color: rgb(216 180 254);
  --dp-border-color: #fff;
  --dp-menu-border-color: #ddd;
  --dp-border-color-hover: rgb(216 180 254);
  --dp-disabled-color: #f6f6f6;
}

.dp__range_between {
  background: var(--dp-hover-color-range);
  border-bottom: var(--dp-hover-color-range);
  border-top: var(--dp-hover-color-range);
}

.dp__overlay_action {
  display: none;
}

.dp__menu {
  padding: 1em;
}

.dp__calendar_header_item {
  justify-items: center;
  align-items: center;
}

.dp__calendar_header {
  font-weight: normal;
}
</style>
