import { z } from "zod";

export const OutdatedNukiCredentialResponseDtoSchema = z.object({
  id: z.number(),
  nukiEmail: z.string().email().nullable(),
  errors: z.array(z.string().nullable()),
});

export type OutdatedNukiCredentialResponseDto = z.infer<
  typeof OutdatedNukiCredentialResponseDtoSchema
>;
