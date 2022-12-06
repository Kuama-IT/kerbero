import { z } from "zod";

export const OutdatedNukiCredentialResponseDtoSchema = z.object({
  id: z.number(),
  nukiEmail: z.string().email().nullable(),
  errors: z.array(z.string().nullable()),
});
export type OutdatedNukiCredentialResponseDto = z.infer<
  typeof OutdatedNukiCredentialResponseDtoSchema
>;

export const CreateNukiCredentialRequestDtoSchema = z.object({
  token: z.string(),
});
export type CreateNukiCredentialRequestDto = z.infer<
  typeof CreateNukiCredentialRequestDtoSchema
>;

export const NukiCredentialResponseDtoSchema = z.object({
  id: z.number(),
  token: z.string(),
});
export type NukiCredentialResponseDto = z.infer<
  typeof NukiCredentialResponseDtoSchema
>;

export const NukiCredentialDraftResponseDtoSchema = z.object({
  redirectUrl: z.string(),
});
export type NukiCredentialDraftResponseDto = z.infer<
  typeof NukiCredentialDraftResponseDtoSchema
>;

export const NukiCredentialListResponseDtoSchema = z.object({
  credentials: z.array(NukiCredentialResponseDtoSchema),
  outdatedCredentials: z.array(OutdatedNukiCredentialResponseDtoSchema),
});
export type NukiCredentialListResponseDto = z.infer<
  typeof NukiCredentialListResponseDtoSchema
>;
