import { httpClient } from "../../shared/http-client/http-client";
import {
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

export const openSmartLockAction = async (
  smartLockId: string,
  request: OpenSmartLockRequestDto
): Promise<void> => {
  OpenSmartLockRequestDtoSchema.parse(request);
  await httpClient.put({
    endpoint: `smart-locks/${smartLockId}/open`,
    request,
  });
};

export const closeSmartLockAction = async (
  smartLockId: string,
  request: OpenSmartLockRequestDto
): Promise<void> => {
  OpenSmartLockRequestDtoSchema.parse(request);
  await httpClient.put({
    endpoint: `smart-locks/${smartLockId}/open`,
    request,
  });
};
