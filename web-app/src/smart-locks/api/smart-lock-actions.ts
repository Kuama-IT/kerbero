import { httpClient } from "@/shared/http-client/http-client";
import {
  CloseSmartLockRequestDto,
  CloseSmartLockRequestDtoSchema,
  OpenSmartLockRequestDto,
  OpenSmartLockRequestDtoSchema,
  SmartLockListResponseDto,
  SmartLockListResponseDtoSchema,
} from "./smart-lock.schemas";

export const listSmartLockAction =
  async (): Promise<SmartLockListResponseDto> => {
    const json = await httpClient.get({ endpoint: "smart-locks" });

    return SmartLockListResponseDtoSchema.parse(json);
  };

export const openSmartLockAction = async (request: {
  smartLockId: string;
  openSmartLockRequestDto: OpenSmartLockRequestDto;
}): Promise<void> => {
  OpenSmartLockRequestDtoSchema.parse(request.openSmartLockRequestDto);
  await httpClient.put({
    endpoint: `smart-locks/${request.smartLockId}/open`,
    request: request.openSmartLockRequestDto,
  });
};

export const closeSmartLockAction = async (request: {
  smartLockId: string;
  closeSmartLockRequestDto: CloseSmartLockRequestDto;
}): Promise<void> => {
  console.log(request);
  CloseSmartLockRequestDtoSchema.parse(request.closeSmartLockRequestDto);
  console.log("parsed");
  await httpClient.put({
    endpoint: `smart-locks/${request.smartLockId}/close`,
    request: request.closeSmartLockRequestDto,
  });
};
