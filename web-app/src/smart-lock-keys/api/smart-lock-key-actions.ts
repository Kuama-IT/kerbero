import { httpClient } from "../../shared/http-client/http-client";
import {
  CreateSmartLockKeyRequestDto,
  CreateSmartLockKeyRequestDtoSchema,
  SmartLockKeyListResponseDto,
  SmartLockKeyListResponseDtoSchema,
  SmartLockKeyResponseDto,
  SmartLockKeyResponseDtoSchema,
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
