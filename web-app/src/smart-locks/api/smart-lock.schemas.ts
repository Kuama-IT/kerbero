import { z } from "zod";
import { SmartLockProviderEnumSchema } from "../../shared/smart-lock-provider.schemas";
import { OutdatedNukiCredentialResponseDtoSchema } from "../../user-credentials/api/nuki-credential.schemas";

const SmartLockStateDtoSchema = z.object({
  description: z.string().nullable(),
  value: z.number(),
});

const SmartLockResponseDtoSchema = z.object({
  id: z.string().nullable(),
  name: z.string().nullable(),
  smartLockProvider: SmartLockProviderEnumSchema,
  credentialId: z.number(),
  state: SmartLockStateDtoSchema,
});

export const SmartLockListResponseDtoSchema = z.object({
  smartLocks: z.array(SmartLockResponseDtoSchema),
  outdatedCredentials: z.array(OutdatedNukiCredentialResponseDtoSchema),
});
export type SmartLockListResponseDto = z.infer<
  typeof SmartLockListResponseDtoSchema
>;

export const OpenSmartLockRequestDtoSchema = z.object({
  credentialsId: z.number(),
  smartLockProvider: z.string(),
});
export type OpenSmartLockRequestDto = z.infer<
  typeof OpenSmartLockRequestDtoSchema
>;

export const CloseSmartLockRequestDtoSchema = z.object({
  credentialsId: z.number(),
  smartLockProvider: z.string(),
});
export type CloseSmartLockRequestDto = z.infer<
  typeof CloseSmartLockRequestDtoSchema
>;
