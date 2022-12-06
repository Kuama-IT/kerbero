import { httpClient } from "../../shared/http-client/http-client";
import {
  CloseSmartLockWithKeyRequestDto,
  CreateSmartLockKeyRequestDto,
  CreateSmartLockKeyRequestDtoSchema,
  OpenSmartLockWithKeyRequestDto,
  OpenSmartLockWithKeyRequestDtoSchema,
  SmartLockKeyListResponseDto,
  SmartLockKeyListResponseDtoSchema,
  SmartLockKeyResponseDto,
  SmartLockKeyResponseDtoSchema,
  UpdateSmartLockKeyRequestDto,
  UpdateSmartLockKeyRequestDtoSchema,
} from "./smart-lock-key.schemas";

export const listSmartLockKeys =
  async (): Promise<SmartLockKeyListResponseDto> => {
    const json = await httpClient.get({ endpoint: "smart-lock-keys" });

    return SmartLockKeyListResponseDtoSchema.parse(json);
  };

export const createSmartLockKey = async (
  request: CreateSmartLockKeyRequestDto
): Promise<SmartLockKeyResponseDto> => {
  CreateSmartLockKeyRequestDtoSchema.parse(request);

  const json = await httpClient.post({ endpoint: "smart-lock-keys", request });

  return SmartLockKeyResponseDtoSchema.parse(json);
};

export const openSmartLockWithKey = async (
  request: OpenSmartLockWithKeyRequestDto
): Promise<void> => {
  OpenSmartLockWithKeyRequestDtoSchema.parse(request);

  await httpClient.post({
    endpoint: "smart-lock-keys/open-smart-lock",
    request,
  });
};

export const closeSmartLockWithKey = async (
  request: CloseSmartLockWithKeyRequestDto
): Promise<void> => {
  CreateSmartLockKeyRequestDtoSchema.parse(request);

  await httpClient.post({
    endpoint: "smart-lock-keys/close-smart-lock",
    request,
  });
};

export const updateSmartLockKey = async (
  smartLockKeyId: string,
  request: UpdateSmartLockKeyRequestDto
): Promise<SmartLockKeyResponseDto> => {
  UpdateSmartLockKeyRequestDtoSchema.parse(request);
  const json = httpClient.put({
    endpoint: `smart-lock-keys/${smartLockKeyId}`,
    request: request,
  });
  return SmartLockKeyResponseDtoSchema.parse(json);
};

export const deleteSmartLockKey = async (
  smartLockKeyId: string
): Promise<SmartLockKeyResponseDto> => {
  const json = httpClient.delete({
    endpoint: `smart-lock-keys/${smartLockKeyId}`,
  });
  return SmartLockKeyResponseDtoSchema.parse(json);
};
